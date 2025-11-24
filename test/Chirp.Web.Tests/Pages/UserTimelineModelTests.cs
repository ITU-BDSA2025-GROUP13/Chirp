using System.Security.Claims;
using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Chirp.Web.Tests.Pages;

public class UserTimelineModelTests
{
    private UserManager<ChirpUser> CreateMockUserManager()
    {
        var userStore = new Mock<IUserStore<ChirpUser>>();
        return new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public void UserTimelineModel_WhenConstructed_InitializesEmptyCheepList()
    {
        var mockCheepService = new Mock<ICheepService>();
        var mockUserService = new Mock<IChirpUserService>();

        var model = CreateModel(mockCheepService, mockUserService);

        Assert.NotNull(model.Cheeps);
        Assert.Empty(model.Cheeps);
    }

    [Fact]
    public void OnGet_WithValidAuthorAndPageNumber_ReturnsPageResultWithCheeps()
    {
        string user = "TestUser";
        var mockCheepService = new Mock<ICheepService>();
        var mockUserService = new Mock<IChirpUserService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("User message 1", "2023-01-01", user, 0, null, new List<CheepDTO>(), "0"),
            new("User message 2", "2023-01-02", user, 1, null, new List<CheepDTO>(), "0")
        };

        mockCheepService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(expectedCheeps);
        var model = CreateModel(mockCheepService, mockUserService);

        model.Author = user;
        model.CurrentPage = 1;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        mockCheepService.Verify(s => s.GetCheepsFromAuthorName(user, 1), Times.Once);
        Assert.Equal(expectedCheeps, model.Cheeps);
    }

    [Fact]
    public void OnGet_WithPageZero_ReturnsPageResultWithFirstPageCheeps()
    {
        string anotherUser = "AnotherUser";
        var mockCheepService = new Mock<ICheepService>();
        var mockUserService = new Mock<IChirpUserService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("First page user message", "2023-01-01", "AnotherUser", 0, null, new List<CheepDTO>(), "0")
        };

        mockCheepService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(expectedCheeps);
        var model = CreateModel(mockCheepService, mockUserService);

        model.Author = anotherUser;
        model.CurrentPage = 1;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        mockCheepService.Verify(s => s.GetCheepsFromAuthorName(anotherUser, 1), Times.Once);
        Assert.Single(model.Cheeps);
        Assert.Equal(anotherUser, model.Cheeps.First().AuthorName);
    }

    [Fact]
    public void OnGet_WhenUserDoesNotExist_ReturnsPageResultWithEmptyCheepList()
    {
        var mockCheepService = new Mock<ICheepService>();
        mockCheepService.Setup(s => s.GetCheepsFromAuthorName("NonExistent", It.IsAny<int>()))
            .Returns(new List<CheepDTO>());
        var mockUserService = new Mock<IChirpUserService>();
        var model = CreateModel(mockCheepService, mockUserService);

        model.Author = "NonExistent";
        model.CurrentPage = 1;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
    }

    [Fact]
    public void OnGet_WhenUsernameHasSpecialCharacters_ReturnsPageResultWithCheeps()
    {
        string user = "User_123";
        var mockCheepService = new Mock<ICheepService>();
        var mockUserService = new Mock<IChirpUserService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("Special user message", "2023-01-01", user, 0, null, new List<CheepDTO>(), "0")
        };

        mockCheepService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(expectedCheeps);
        var model = CreateModel(mockCheepService, mockUserService);

        model.Author = user;
        model.CurrentPage = 1;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Single(model.Cheeps);
    }

    private static UserTimelineModel CreateModel(Mock<ICheepService> cheepServiceMock, Mock<IChirpUserService> userServiceMock, string? userName = null)
    {
        var mockUserManager = new Mock<UserManager<ChirpUser>>(Mock.Of<IUserStore<ChirpUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        var model = new UserTimelineModel(cheepServiceMock.Object, userServiceMock.Object, mockUserManager.Object)
        {
            PageContext = new PageContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var identity = userName != null
            ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "TestAuth")
            : new ClaimsIdentity();

        model.PageContext.HttpContext.User = new ClaimsPrincipal(identity);

        return model;
    }
}
