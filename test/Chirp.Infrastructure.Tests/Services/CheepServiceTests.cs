using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Chirp.Infrastructure.Tests.Services;

public class CheepServiceTests
{
    [Fact]
    public void GetMainPageCheeps_ReturnsDtoWithFallbackAndReplies()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var anonymousAuthor = new ChirpUser { Id = "anon" };
        var replyAuthor = new ChirpUser { Id = "reply", UserName = "Responder" };
        var reply = new Cheep
        {
            CheepId = 2,
            AuthorId = replyAuthor.Id,
            Author = replyAuthor,
            Text = "Hi",
            TimeStamp = DateTime.UtcNow,
            Replies = []
        };
        var root = new Cheep
        {
            CheepId = 1,
            AuthorId = anonymousAuthor.Id,
            Author = anonymousAuthor,
            Text = "Hello",
            TimeStamp = DateTime.UtcNow,
            Replies = [reply]
        };
        reply.ParentCheep = root;

        repoMock.Setup(r => r.GetMainPage(1)).ReturnsAsync(new List<Cheep> { root });

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<CheepDTO> result = service.GetMainPageCheeps(1);

        CheepDTO dto = Assert.Single(result);
        Assert.Equal("Deleted User", dto.AuthorName);
        CheepDTO replyDto = Assert.Single(dto.Replies);
        Assert.Equal("Responder", replyDto.AuthorName);
        repoMock.Verify(r => r.GetMainPage(1), Times.Once);
    }

    [Fact]
    public void GetCheepsFromAuthorName_UserMissing_ReturnsEmpty()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(m => m.FindByNameAsync("ghost")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<CheepDTO> result = service.GetCheepsFromAuthorName("ghost");

        Assert.Empty(result);
        repoMock.Verify(r => r.GetAuthorPage(It.IsAny<ChirpUser>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetCheepsFromAuthorName_UserFound_ReturnsDtos()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var author = new ChirpUser { Id = "author", UserName = "Author" };
        userManagerMock.Setup(m => m.FindByNameAsync(author.UserName!)).ReturnsAsync(author);

        var cheep = new Cheep
        {
            CheepId = 1,
            AuthorId = author.Id,
            Author = author,
            Text = "Message",
            TimeStamp = DateTime.UtcNow,
            Replies = []
        };
        repoMock.Setup(r => r.GetAuthorPage(author, 2)).ReturnsAsync(new List<Cheep> { cheep });

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<CheepDTO> result = service.GetCheepsFromAuthorName(author.UserName!, 2);

        CheepDTO dto = Assert.Single(result);
        Assert.Equal(cheep.CheepId, dto.CheepId);
        repoMock.Verify(r => r.GetAuthorPage(author, 2), Times.Once);
    }

    [Fact]
    public async Task GetListOfFollowers_UserMissing_ReturnsEmpty()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(m => m.FindByNameAsync("ghost")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<ChirpUser> result = await service.GetListOfFollowers("ghost");

        Assert.Empty(result);
        repoMock.Verify(r => r.GetListOfFollowers(It.IsAny<ChirpUser>()), Times.Never);
    }

    [Fact]
    public async Task GetListOfFollowers_UserFound_ReturnsRepositoryResult()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var user = new ChirpUser { Id = "user", UserName = "User" };
        userManagerMock.Setup(m => m.FindByNameAsync(user.UserName!)).ReturnsAsync(user);

        var follower = new ChirpUser { Id = "follower", UserName = "Follower" };
        repoMock.Setup(r => r.GetListOfFollowers(user)).ReturnsAsync(new List<ChirpUser> { follower });

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<ChirpUser> result = await service.GetListOfFollowers(user.UserName!);

        ChirpUser loaded = Assert.Single(result);
        Assert.Equal(follower.Id, loaded.Id);
    }

    [Fact]
    public void GetListOfNamesOfFollowedUsers_FiltersInvalidNames()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var user = new ChirpUser { Id = "user", UserName = "User" };
        userManagerMock.Setup(m => m.FindByNameAsync(user.UserName!)).ReturnsAsync(user);

        List<ChirpUser> followed =
        [
            new ChirpUser { UserName = "Friend" },
            new ChirpUser { UserName = null },
            new ChirpUser { UserName = "" }
        ];
        repoMock.Setup(r => r.GetListOfFollowers(user)).ReturnsAsync(followed);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<string> result = service.GetListOfNamesOfFollowedUsers(user.UserName!);

        Assert.Equal(2, result.Count);
        Assert.Contains("Friend", result);
        Assert.Contains(string.Empty, result);
    }

    [Fact]
    public void GetOwnPrivateTimeline_UserMissing_ReturnsEmpty()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(m => m.FindByNameAsync("ghost")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<CheepDTO> result = service.GetOwnPrivateTimeline("ghost");

        Assert.Empty(result);
        repoMock.Verify(r => r.GetPrivateTimelineCheeps(It.IsAny<ChirpUser>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetOwnPrivateTimeline_UserFound_ReturnsDtos()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var user = new ChirpUser { Id = "user", UserName = "User" };
        userManagerMock.Setup(m => m.FindByNameAsync(user.UserName!)).ReturnsAsync(user);

        var cheep = new Cheep
        {
            CheepId = 1,
            AuthorId = user.Id,
            Author = user,
            Text = "Timeline",
            TimeStamp = DateTime.UtcNow,
            Replies = []
        };
        repoMock.Setup(r => r.GetPrivateTimelineCheeps(user, 1)).ReturnsAsync(new List<Cheep> { cheep });

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<CheepDTO> result = service.GetOwnPrivateTimeline(user.UserName!, 1);

        CheepDTO dto = Assert.Single(result);
        Assert.Equal(cheep.Text, dto.Text);
    }

    [Fact]
    public void GetAuthorIDFromName_Found_ReturnsId()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var author = new ChirpUser { Id = "123", UserName = "Author" };
        userManagerMock.Setup(m => m.FindByNameAsync(author.UserName!)).ReturnsAsync(author);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        string id = service.GetAuthorIDFromName(author.UserName!);

        Assert.Equal(author.Id, id);
    }

    [Fact]
    public void GetAuthorIDFromName_Missing_Throws()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByNameAsync("ghost")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        Exception ex = Assert.Throws<Exception>(() => service.GetAuthorIDFromName("ghost"));
        Assert.Contains("ghost", ex.Message);
    }

    [Fact]
    public void PostCheep_UserMissing_Throws()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByIdAsync("missing")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        Exception ex = Assert.Throws<Exception>(() => service.PostCheep("text", "missing"));
        Assert.Contains("missing", ex.Message);
    }

    [Fact]
    public void PostCheep_UserFound_InsertsCheep()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var author = new ChirpUser { Id = "author", UserName = "Author" };
        userManagerMock.Setup(m => m.FindByIdAsync(author.Id)).ReturnsAsync(author);

        repoMock.Setup(r => r.InsertCheep(It.IsAny<Cheep>())).Returns(Task.CompletedTask);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        service.PostCheep("message", author.Id);

        repoMock.Verify(r => r.InsertCheep(It.Is<Cheep>(c =>
            c.AuthorId == author.Id &&
            c.Author == author &&
            c.Text == "message" &&
            c.TimeStamp != default
        )), Times.Once);
    }

    [Fact]
    public void DeleteCheep_NotFound_Throws()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        repoMock.Setup(r => r.GetCheepById(1)).ReturnsAsync((Cheep?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        Exception ex = Assert.Throws<Exception>(() => service.DeleteCheep(1));
        Assert.Contains("1", ex.Message);
        repoMock.Verify(r => r.DeleteCheep(It.IsAny<Cheep>()), Times.Never);
    }

    [Fact]
    public void DeleteCheep_Found_Deletes()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var cheep = new Cheep
        {
            CheepId = 1,
            AuthorId = "author",
            Author = new ChirpUser { Id = "author", UserName = "Author" },
            Text = "message",
            TimeStamp = DateTime.UtcNow,
            Replies = []
        };

        repoMock.Setup(r => r.GetCheepById(1)).ReturnsAsync(cheep);
        repoMock.Setup(r => r.DeleteCheep(cheep)).Returns(Task.CompletedTask).Verifiable();

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        service.DeleteCheep(1);

        repoMock.Verify();
    }

    [Fact]
    public void ReplyToCheep_ParentMissing_Throws()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        repoMock.Setup(r => r.GetCheepById(1)).ReturnsAsync((Cheep?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        var author = new ChirpUser { Id = "author", UserName = "Author" };

        Assert.Throws<ArgumentNullException>(() => service.ReplyToCheep(1, "reply", author));
        repoMock.Verify(r => r.InsertCheep(It.IsAny<Cheep>()), Times.Never);
    }

    [Fact]
    public void ReplyToCheep_InsertsReply()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        var parent = new Cheep
        {
            CheepId = 1,
            AuthorId = "parent",
            Author = new ChirpUser { Id = "parent", UserName = "Parent" },
            Text = "parent",
            TimeStamp = DateTime.UtcNow,
            Replies = []
        };
        repoMock.Setup(r => r.GetCheepById(parent.CheepId)).ReturnsAsync(parent);
        repoMock.Setup(r => r.InsertCheep(It.IsAny<Cheep>())).Returns(Task.CompletedTask);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);
        var author = new ChirpUser { Id = "author", UserName = "Author" };

        service.ReplyToCheep(parent.CheepId, "reply", author);

        repoMock.Verify(r => r.InsertCheep(It.Is<Cheep>(c =>
            c.AuthorId == author.Id &&
            c.Author == author &&
            c.Text == "reply" &&
            c.ParentCheep == parent &&
            c.TimeStamp != default
        )), Times.Once);
    }

    [Fact]
    public void EditCheep_DelegatesToRepository()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();
        repoMock.Setup(r => r.EditCheepById(1, "new")).Returns(Task.CompletedTask).Verifiable();

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        service.EditCheep(1, "new");

        repoMock.Verify();
    }

    [Fact]
    public void GetCheepsFromAuthorID_UserMissing_ReturnsEmpty()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByIdAsync("123")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<CheepDTO> result = service.GetCheepsFromAuthorID("123");

        Assert.Empty(result);
        repoMock.Verify(r => r.GetAuthorPage(It.IsAny<ChirpUser>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetCheepsFromAuthorEmail_UserMissing_ReturnsEmpty()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByEmailAsync("mail@test.com")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        List<CheepDTO> result = service.GetCheepsFromAuthorEmail("mail@test.com");

        Assert.Empty(result);
        repoMock.Verify(r => r.GetAuthorPage(It.IsAny<ChirpUser>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetAllCheepsFromAuthorName_UserMissing_Throws()
    {
        Mock<ICheepRepository> repoMock = new Mock<ICheepRepository>();
        Mock<UserManager<ChirpUser>> userManagerMock = CreateUserManagerMock();

        userManagerMock.Setup(m => m.FindByNameAsync("ghost")).ReturnsAsync((ChirpUser?)null);

        var service = new CheepService(repoMock.Object, userManagerMock.Object);

        Exception ex = Assert.Throws<Exception>(() => service.GetAllCheepsFromAuthorName("ghost"));
        Assert.Contains("ghost", ex.Message);
    }

    private static Mock<UserManager<ChirpUser>> CreateUserManagerMock()
    {
        var storeMock = new Mock<IUserStore<ChirpUser>>();
        return new Mock<UserManager<ChirpUser>>(
            storeMock.Object,
            null!,
            null!,
            Enumerable.Empty<IUserValidator<ChirpUser>>(),
            Enumerable.Empty<IPasswordValidator<ChirpUser>>(),
            null!,
            null!,
            null!,
            null!);
    }
}
