using Chirp.Domain;
using FluentAssertions;
using Moq;

namespace Chirp.Razor.Tests;

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

        var mockRepository = new Mock<ICheepRepository>();

        mockRepository
            .Setup(c => c.GetMainPageAsync(0))
            .ReturnsAsync(cheeps);

        var service = new CheepService(mockRepository.Object);

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

        string name1 = "TestUser1";
        string name2 = "TestUser2";

        var author1 = new Author { AuthorId = 1, Name = name1, Email = "test1@test.com" };
        var author2 = new Author { AuthorId = 2, Name = name2, Email = "test2@test.com" };
        var authors = new List<Author> { author1, author2 };

        var author2Cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 3, AuthorId = 2, Author = author2, Text = text3, TimeStamp = DateTime.Now.AddHours(-3) },
            new Cheep { CheepId = 2, AuthorId = 2, Author = author2, Text = text2, TimeStamp = DateTime.Now.AddHours(-4) },
            new Cheep { CheepId = 1, AuthorId = 2, Author = author2, Text = text1, TimeStamp = DateTime.Now.AddHours(-5) }
        };

        var author2_autour = new Author
        {
            AuthorId = author2.AuthorId,
            Name = author2.Name,
            Email = author2.Email,
            Cheeps = author2Cheeps
        };


        var mockRepository = new Mock<ICheepRepository>();

        var mockAuthor2CheepSet = author2Cheeps;
        var mockAuthorSet = authors;

        mockRepository
            .Setup(c => c.GetPageFromAuthorAsync(It.IsAny<string>(), 0))
            .ReturnsAsync(author2_autour);

        var service = new CheepService(mockRepository.Object);

        var result = service.GetCheepsFromAuthorName(name2);

        result.Should().HaveCount(author2Cheeps.Count());
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

        var mockRepository = new Mock<ICheepRepository>();

        mockRepository
            .Setup(c => c.GetAuthorFromAuthorID(It.IsAny<int>()))
            .Returns(author1);

        mockRepository
            .Setup(c => c.InsertCheep(It.IsAny<Cheep>()))
            .Callback<Cheep>(c => cheeps.Push(c))
            .Returns(Task.CompletedTask);

        mockRepository
            .Setup(c => c.GetMainPageAsync(It.IsAny<int>()))
            .ReturnsAsync(() => cheeps);

        var service = new CheepService(mockRepository.Object);
        service.PostCheep(text2, author1.AuthorId);

        var result = service.GetMainPageCheeps(0);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Text.Should().Be(text2);
        result.Last().Text.Should().Be(text1);
    }
}
