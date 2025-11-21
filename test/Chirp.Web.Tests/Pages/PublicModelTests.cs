using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace Chirp.Web.Tests.Pages;

public class PublicModelTests
{
    [Fact]
    public void PublicModel_WhenConstructed_InitializesEmptyCheepList()
    {
        var mockService = new Mock<ICheepService>();
        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        var model = new PublicModel(mockService.Object, userManager);

        Assert.NotNull(model.Cheeps);
        Assert.Empty(model.Cheeps);
    }

    [Fact]
    public void OnGet_WithPageNumber_ReturnsPageResultWithCheeps()
    {
        var mockService = new Mock<ICheepService>();
        var expectedCheeps = new List<CheepDTO>
        {
            new("Public message 1", "2023-01-01", "User1", 0, null, new List<CheepDTO>()),
            new("Public message 2", "2023-01-02", "User2", 1, null, new List<CheepDTO>())
        };

        mockService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>())).Returns(expectedCheeps);
        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        var model = new PublicModel(mockService.Object, userManager);

        model.CurrentPage = 1;
        var result = model.OnGet();

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
            new(text, "2023-01-01", "User1", 0, null, new List<CheepDTO>())
        };

        mockService.Setup(s => s.GetMainPageCheeps(It.IsAny<int>()))
           .Returns(expectedCheeps);
        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        var model = new PublicModel(mockService.Object, userManager);

        model.CurrentPage = 0;
        var result = model.OnGet();

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
        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        var model = new PublicModel(mockService.Object, userManager);

        model.CurrentPage = 5;
        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
        Assert.Empty(model.Cheeps);
    }
}
