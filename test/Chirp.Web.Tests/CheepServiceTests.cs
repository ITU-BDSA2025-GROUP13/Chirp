using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Chirp.Web.Tests;

public class CheepServiceTests
{
    [Fact]
    public void GetCheeps_WhenCalled_ReturnsCheepsInDescendingOrder()
    {
        string text3 = "Newest cheep";
        string text2 = "Older cheep";
        string text1 = "Oldest cheep";

        var author = new ChirpUser() { Id = "1", UserName = "TestUser", Email = "test@test.com" };
        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 3, AuthorId = "1", Author = author, Text = text3, TimeStamp = DateTime.Now },
            new Cheep { CheepId = 2, AuthorId = "1", Author = author, Text = text2, TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep { CheepId = 1, AuthorId = "1", Author = author, Text = text1, TimeStamp = DateTime.Now.AddHours(-2) }
        };

        var mockCheepRepo = new Mock<ICheepRepository>();

        mockCheepRepo
            .Setup(c => c.GetMainPage(0))
            .ReturnsAsync(cheeps);

        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var service = new CheepService(mockCheepRepo.Object, userManager);

        var result = service.GetMainPageCheeps();

        Assert.Equal(cheeps.Count, result.Count);
        Assert.Equal(text3, result.First().Text);
        Assert.Equal(text1, result.Last().Text);
    }

    [Fact]
    public void GetCheepsFromAuthor_WithValidAuthorName_ReturnsAuthorsCheepsInDescendingOrder()
    {
        string text3 = "Newest cheep";
        string text2 = "Older cheep";
        string text1 = "Oldest cheep";

        string name2 = "TestUser2";
        var author2 = new ChirpUser() { Id = "2", UserName = name2, Email = "test2@test.com" };

        var author2Cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 3, AuthorId = "2", Author = author2, Text = text3, TimeStamp = DateTime.Now.AddHours(-3) },
            new Cheep { CheepId = 2, AuthorId = "2", Author = author2, Text = text2, TimeStamp = DateTime.Now.AddHours(-4) },
            new Cheep { CheepId = 1, AuthorId = "2", Author = author2, Text = text1, TimeStamp = DateTime.Now.AddHours(-5) }
        };
        author2.Cheeps = author2Cheeps;

        var mockCheepRepo = new Mock<ICheepRepository>();
        mockCheepRepo
            .Setup(c => c.GetAuthorPage(It.IsAny<ChirpUser>(), It.IsAny<int>()))
            .ReturnsAsync(author2Cheeps);

        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        userStore
            .Setup(c => c.FindByNameAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(author2);
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var service = new CheepService(mockCheepRepo.Object, userManager);

        var result = service.GetCheepsFromAuthorName(name2);
        Assert.Equal(author2Cheeps.Count, result.Count());
        Assert.Equal(text3, result.First().Text);
        Assert.Equal(text1, result.Last().Text);
    }

    [Fact]
    public void PostCheep_WithValidMessage_AddsCheepAndIsReturnedFirst()
    {
        string text2 = "Newest cheep";
        string text1 = "Oldest cheep";

        var author1 = new ChirpUser { Id = "1", UserName = "TestUser1", Email = "test1@test.com" };
        var authors = new List<ChirpUser> { author1 };

        var cheeps = new Stack<Cheep>();
        cheeps.Push(new Cheep { CheepId = 1, AuthorId = "1", Author = author1, Text = text1, TimeStamp = DateTime.Now.AddHours(-1) });

        var mockCheepRepo = new Mock<ICheepRepository>();

        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        userStore
            .Setup(c => c.FindByIdAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(author1);
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        mockCheepRepo
            .Setup(c => c.InsertCheep(It.IsAny<Cheep>()))
            .Callback<Cheep>(c => cheeps.Push(c))
            .Returns(Task.CompletedTask);

        mockCheepRepo
            .Setup(c => c.GetMainPage(It.IsAny<int>()))
            .ReturnsAsync(() => cheeps);

        var service = new CheepService(mockCheepRepo.Object, userManager);
        service.PostCheep(text2, author1.Id);

        var result = service.GetMainPageCheeps(0);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(text2, result.First().Text);
        Assert.Equal(text1, result.Last().Text);
    }

    [Fact]
    public void EditCheep_WithValidCheepId_EditsCheepMessage()
    {
        string text1 = "original cheep";
        string text2 = "edited cheep";

        var author1 = new ChirpUser { Id = "1", UserName = "TestUser1", Email = "test1@test.com" };

        var cheeps = new List<Cheep>();
        var cheep = new Cheep { CheepId = 1, AuthorId = "1", Author = author1, Text = text1, TimeStamp = DateTime.Now.AddHours(-1) };
        cheeps.Add(cheep);

        var mockCheepRepo = new Mock<ICheepRepository>();

        Mock<IUserStore<ChirpUser>> userStore = new Mock<IUserStore<ChirpUser>>();
        UserManager<ChirpUser> userManager = new UserManager<ChirpUser>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        mockCheepRepo
            .Setup(c => c.EditCheepById(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = new CheepService(mockCheepRepo.Object, userManager);
        service.EditCheep(cheep.CheepId, text2);
    }
}
