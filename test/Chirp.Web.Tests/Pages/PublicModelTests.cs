using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
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

        Assert.NotNull(model.Cheeps);
        Assert.Empty(model.Cheeps);
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

        Assert.IsType<PageResult>(result);
        mockService.Verify(s => s.GetMainPageCheeps(1), Times.Once);
        Assert.Equal(expectedCheeps, model.Cheeps);
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

        Assert.IsType<PageResult>(result);
        mockService.Verify(s => s.GetMainPageCheeps(0), Times.Once);
        Assert.Single(model.Cheeps);
        Assert.Equal(text, model.Cheeps.First().Text);
    }

    [Fact]
    public void OnGet_WhenServiceReturnsNoCheeps_ReturnsPageResultWithEmptyCheepList()
    {
        var mockService = new Mock<ICheepService>();
        mockService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>())).Returns(new List<CheepDTO>());
        var model = new PublicModel(mockService.Object);

        var result = model.OnGet(5);

        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
    }
}
