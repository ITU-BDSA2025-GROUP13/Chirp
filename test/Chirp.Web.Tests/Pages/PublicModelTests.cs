using System.Security.Claims;
using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Chirp.Web.Tests.Pages;

public class PublicModelTests
{
    [Fact]
    public void PublicModel_WhenConstructed_InitializesEmptyCheepList()
    {
        var mockCheepService = new Mock<ICheepService>();
        var mockUserService = new Mock<IChirpUserService>();
    var model = CreateModel(mockCheepService, mockUserService);

        Assert.NotNull(model.Cheeps);
        Assert.Empty(model.Cheeps);
    }

    [Fact]
    public void OnGet_WithPageNumber_ReturnsPageResultWithCheeps()
    {
        var mockCheepService = new Mock<ICheepService>();
        var mockUserService = new Mock<IChirpUserService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("Public message 1", "2023-01-01", "User1", 0, null, new List<CheepDTO>()),
            new("Public message 2", "2023-01-02", "User2", 1, null, new List<CheepDTO>())
        };

        mockCheepService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>())).Returns(expectedCheeps);
    var model = CreateModel(mockCheepService, mockUserService);

        model.CurrentPage = 1;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        mockCheepService.Verify(s => s.GetMainPageCheeps(1), Times.Once);
        Assert.Equal(expectedCheeps, model.Cheeps);
    }

    [Fact]
    public void OnGet_WithPageZero_ReturnsPageResultWithFirstPageCheeps()
    {
        string text = "First page message";
        var mockCheepService = new Mock<ICheepService>();
        var mockUserService = new Mock<IChirpUserService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new(text, "2023-01-01", "User1", 0, null, new List<CheepDTO>())
        };

        mockCheepService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>()))
           .Returns(expectedCheeps);
    var model = CreateModel(mockCheepService, mockUserService);
        model.CurrentPage = 0;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        mockCheepService.Verify(s => s.GetMainPageCheeps(0), Times.Once);
        Assert.Single(model.Cheeps);
        Assert.Equal(text, model.Cheeps.First().Text);
    }

    [Fact]
    public void OnGet_WhenServiceReturnsNoCheeps_ReturnsPageResultWithEmptyCheepList()
    {
        var mockCheepService = new Mock<ICheepService>();
        mockCheepService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>())).Returns(new List<CheepDTO>());
        var mockUserService = new Mock<IChirpUserService>();
    var model = CreateModel(mockCheepService, mockUserService);

        model.CurrentPage = 5;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
    }

        private static PublicModel CreateModel(Mock<ICheepService> cheepService, Mock<IChirpUserService> userService, string? userName = null)
        {
            var userStore = new Mock<IUserStore<ChirpUser>>();
            var userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var model = new PublicModel(cheepService.Object, userService.Object, userManager)
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            model.PageContext.HttpContext.User = userName != null
                ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "TestAuth"))
                : new ClaimsPrincipal(new ClaimsIdentity());

            return model;
        }
}