using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Chirp.Web.Tests.Pages;

public class PublicModelTests
{
    [Fact]
    public void PublicModel_WhenConstructed_InitializesEmptyCheepList()
    {
        var mockService = new Mock<ICheepService>();

        var model = new PublicModel(mockService.Object);

        model.Cheeps.Should().NotBeNull();
        model.Cheeps.Should().BeEmpty();
    }

    [Fact]
    public void OnGet_WithPageNumber_ReturnsPageResultWithCheeps()
    {
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("Public message 1", "2023-01-01", "User1"),
            new("Public message 2", "2023-01-02", "User2")
        };

        mockService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>())).Returns(expectedCheeps);
        var model = new PublicModel(mockService.Object);

        var result = model.OnGet(1);

        result.Should().BeOfType<PageResult>();
        mockService.Verify(s => s.GetMainPageCheeps(1), Times.Once);
        model.Cheeps.Should().BeEquivalentTo(expectedCheeps);
    }

    [Fact]
    public void OnGet_WithPageZero_ReturnsPageResultWithFirstPageCheeps()
    {
        string text = "First page message";
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new(text, "2023-01-01", "User1")
        };

        mockService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>()))
           .Returns(expectedCheeps);
        var model = new PublicModel(mockService.Object);

        var result = model.OnGet(0);

        result.Should().BeOfType<PageResult>();
        mockService.Verify(s => s.GetMainPageCheeps(0), Times.Once);
        model.Cheeps.Should().HaveCount(1);
        model.Cheeps.First().Text.Should().Be(text);
    }

    [Fact]
    public void OnGet_WhenServiceReturnsNoCheeps_ReturnsPageResultWithEmptyCheepList()
    {
        var mockService = new Mock<ICheepService>();
        mockService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>())).Returns(new List<CheepDTO>());
        var model = new PublicModel(mockService.Object);

        var result = model.OnGet(5);

        result.Should().BeOfType<PageResult>();
        model.Cheeps.Should().BeEmpty();
    }
}
