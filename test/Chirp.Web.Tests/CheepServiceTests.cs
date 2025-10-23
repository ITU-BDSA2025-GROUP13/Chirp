using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using FluentAssertions;
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

        var author = new Author { AuthorId = 1, Name = "TestUser", Email = "test@test.com" };
        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 3, AuthorId = 1, Author = author, Text = text3, TimeStamp = DateTime.Now },
            new Cheep { CheepId = 2, AuthorId = 1, Author = author, Text = text2, TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep { CheepId = 1, AuthorId = 1, Author = author, Text = text1, TimeStamp = DateTime.Now.AddHours(-2) }
        };

        var mockCheepRepo = new Mock<ICheepRepository>();
        var mockAuthorRepo = new Mock<IAuthorRepository>();

        mockCheepRepo
            .Setup(c => c.GetMainPage(0))
            .ReturnsAsync(cheeps);

        var service = new CheepService(mockCheepRepo.Object,  mockAuthorRepo.Object);

        var result = service.GetMainPageCheeps(0);

        result.Should().HaveCount(cheeps.Count());
        result.First().Text.Should().Be(text3);
        result.Last().Text.Should().Be(text1);
    }

    [Fact]
    public void GetCheepsFromAuthor_WithValidAuthorName_ReturnsAuthorsCheepsInDescendingOrder()
    {
        string text3 = "Newest cheep";
        string text2 = "Older cheep";
        string text1 = "Oldest cheep";
        
        string name2 = "TestUser2";
        var author2 = new Author { AuthorId = 2, Name = name2, Email = "test2@test.com" };
        
        var author2Cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 3, AuthorId = 2, Author = author2, Text = text3, TimeStamp = DateTime.Now.AddHours(-3) },
            new Cheep { CheepId = 2, AuthorId = 2, Author = author2, Text = text2, TimeStamp = DateTime.Now.AddHours(-4) },
            new Cheep { CheepId = 1, AuthorId = 2, Author = author2, Text = text1, TimeStamp = DateTime.Now.AddHours(-5) }
        };
        author2.Cheeps = author2Cheeps;
        
        var mockCheepRepo = new Mock<ICheepRepository>();
        var mockAuthorRepo =  new Mock<IAuthorRepository>();
        mockAuthorRepo
            .Setup(c => c.GetAuthorByName(It.IsAny<string>()))
            .Returns(author2);

        var service = new CheepService(mockCheepRepo.Object, mockAuthorRepo.Object);

        var result = service.GetCheepsFromAuthorName(name2);
        result.Should().HaveCount(author2Cheeps.Count);
        result.First().Text.Should().Be(text3);
        result.Last().Text.Should().Be(text1);
    }

    [Fact]
    public void PostCheep_WithValidMessage_AddsCheepAndIsReturnedFirst()
    {
        string text2 = "Newest cheep";
        string text1 = "Oldest cheep";

        var author1 = new Author { AuthorId = 1, Name = "TestUser1", Email = "test1@test.com" };
        var authors = new List<Author> { author1 };

        var cheeps = new Stack<Cheep>();
        cheeps.Push(new Cheep { CheepId = 1, AuthorId = 1, Author = author1, Text = text1, TimeStamp = DateTime.Now.AddHours(-1) });

        var mockCheepRepo = new Mock<ICheepRepository>();
        var mockAuthorRepo = new Mock<IAuthorRepository>();
        
        mockAuthorRepo
            .Setup(c => c.GetAuthorByID(It.IsAny<int>()))
            .Returns(author1);

        mockCheepRepo
            .Setup(c => c.InsertCheep(It.IsAny<Cheep>()))
            .Callback<Cheep>(c => cheeps.Push(c))
            .Returns(Task.CompletedTask);

        mockCheepRepo
            .Setup(c => c.GetMainPage(It.IsAny<int>()))
            .ReturnsAsync(() => cheeps);

        var service = new CheepService(mockCheepRepo.Object, mockAuthorRepo.Object);
        service.PostCheep(text2, author1.AuthorId);

        var result = service.GetMainPageCheeps(0);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Text.Should().Be(text2);
        result.Last().Text.Should().Be(text1);
    }
}
