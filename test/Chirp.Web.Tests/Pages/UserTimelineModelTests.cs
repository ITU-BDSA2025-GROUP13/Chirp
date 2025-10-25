using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using FluentAssertions;
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

        model.Cheeps.Should().NotBeNull();
        model.Cheeps.Should().BeEmpty();
    }

    [Fact]
    public void OnGet_WithValidAuthorAndPageNumber_ReturnsPageResultWithCheeps()
    {
        string user = "TestUser";
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("User message 1", "2023-01-01", user),
            new("User message 2", "2023-01-02", user)
        };

        mockService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
           .Returns(expectedCheeps); 
        var model = new UserTimelineModel(mockService.Object);

        var result = model.OnGet(user, 0);

        result.Should().BeOfType<PageResult>();
        mockService.Verify(s => s.GetCheepsFromAuthorName(user, 0), Times.Once);
        model.Cheeps.Should().BeEquivalentTo(expectedCheeps);
    }

    [Fact]
    public void OnGet_WithPageZero_ReturnsPageResultWithFirstPageCheeps()
    {
        string anotherUser = "AnotherUser";
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("First page user message", "2023-01-01", "AnotherUser")
        };

        mockService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
           .Returns(expectedCheeps); 
        var model = new UserTimelineModel(mockService.Object);

        var result = model.OnGet(anotherUser, 0);

        result.Should().BeOfType<PageResult>();
        mockService.Verify(s => s.GetCheepsFromAuthorName(anotherUser, 0), Times.Once);
        model.Cheeps.Should().HaveCount(1);
        model.Cheeps.First().AuthorName.Should().Be(anotherUser);
    }

    [Fact]
    public void OnGet_WhenUserDoesNotExist_ReturnsPageResultWithEmptyCheepList()
    {
        var mockService = new Mock<ICheepService>();
        mockService.Setup(s => s.GetCheepsFromAuthorName("NonExistent", It.IsAny<int>()))
                  .Returns(new List<CheepDTO>());
        var model = new UserTimelineModel(mockService.Object);

        var result = model.OnGet("NonExistent", 1);

        result.Should().BeOfType<PageResult>();
        model.Cheeps.Should().BeEmpty();
    }

    [Fact]
    public void OnGet_WhenUsernameHasSpecialCharacters_ReturnsPageResultWithCheeps()
    {
        string user = "User_123";
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("Special user message", "2023-01-01", user)
        };

        mockService.Setup(s => s.GetCheepsFromAuthorName(It.IsAny<string>(), It.IsAny<int>()))
           .Returns(expectedCheeps); 
        var model = new UserTimelineModel(mockService.Object);
        

        var result = model.OnGet(user, 0);

        result.Should().BeOfType<PageResult>();
        model.Cheeps.Should().HaveCount(1);
    }
}
