using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Chirp.Web.Tests.Pages;

public class UserTimelineModelTests
{
    [Fact]
    public void UserTimelineModel_WhenConstructed_InitializesEmptyCheepList()
    {
        var mockService = new Mock<ICheepService>();

        var model = new UserTimelineModel(mockService.Object);

        Assert.NotNull(model.Cheeps);
        Assert.Empty(model.Cheeps);
    }

    [Fact]
    public void OnGet_WithValidAuthorAndPageNumber_ReturnsPageResultWithCheeps()
    {
        string user = "TestUser";
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("User message 1", "2023-01-01", user, 0, null, new List<CheepDTO>()),
            new("User message 2", "2023-01-02", user, 1, null, new List<CheepDTO>())
        };

        mockService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
           .Returns(expectedCheeps);
        var model = new UserTimelineModel(mockService.Object);

        model.Author = user;
        model.CurrentPage = 0;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        mockService.Verify(s => s.GetCheepsFromAuthorName(user, 0), Times.Once);
        Assert.Equal(expectedCheeps, model.Cheeps);
    }

    [Fact]
    public void OnGet_WithPageZero_ReturnsPageResultWithFirstPageCheeps()
    {
        string anotherUser = "AnotherUser";
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("First page user message", "2023-01-01", "AnotherUser", 0, null, new List<CheepDTO>())
        };

        mockService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
           .Returns(expectedCheeps);
        var model = new UserTimelineModel(mockService.Object);

        model.Author = anotherUser;
        model.CurrentPage = 0;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        mockService.Verify(s => s.GetCheepsFromAuthorName(anotherUser, 0), Times.Once);
        Assert.Single(model.Cheeps);
        Assert.Equal(anotherUser, model.Cheeps.First().AuthorName);
    }

    [Fact]
    public void OnGet_WhenUserDoesNotExist_ReturnsPageResultWithEmptyCheepList()
    {
        var mockService = new Mock<ICheepService>();
        mockService.Setup(s => s.GetCheepsFromAuthorName("NonExistent", It.IsAny<int>()))
                  .Returns(new List<CheepDTO>());
        var model = new UserTimelineModel(mockService.Object);

        model.Author = "NonExistent";
        model.CurrentPage = 0;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
    }

    [Fact]
    public void OnGet_WhenUsernameHasSpecialCharacters_ReturnsPageResultWithCheeps()
    {
        string user = "User_123";
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("Special user message", "2023-01-01", user, 0, null, new List<CheepDTO>())
        };

        mockService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
           .Returns(expectedCheeps);
        var model = new UserTimelineModel(mockService.Object);


        model.Author = user;
        model.CurrentPage = 0;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Single(model.Cheeps);
    }
}
