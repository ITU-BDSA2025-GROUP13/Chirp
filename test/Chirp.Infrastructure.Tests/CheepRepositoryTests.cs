using Chirp.Infrastructure;
using FluentAssertions;
using Xunit;
using Chirp.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using System.Linq;

namespace Chirp.Infrastructure.Tests.Repository;

public class CheepRepositoryTests
{
    [Fact]
    public async Task ReadPageAsync_ShouldReturnCheepsOrderedByTimestampDescending()
    {
        // Arrange
        var author = new Author { AuthorId = 1, Name = "TestUser", Email = "test@test.com" };
        var cheeps = new List<Cheep>
        {
            new() { CheepId = 3, AuthorId = 1, Author = author, Text = "Newest cheep", TimeStamp = DateTime.Now },
            new() { CheepId = 2, AuthorId = 1, Author = author, Text = "Older cheep", TimeStamp = DateTime.Now.AddHours(-1) },
            new() { CheepId = 1, AuthorId = 1, Author = author, Text = "Oldest cheep", TimeStamp = DateTime.Now.AddHours(-2) }
        };

        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        var repository = new CheepRepository(mockContext.Object);

        var result = await repository.ReadPageAsync(0);

        result.Should().HaveCount(3);
        result.First().Text.Should().Be("Newest cheep");
        result.Last().Text.Should().Be("Oldest cheep");
    }

    [Fact]
    public async Task ReadPageFromAuthorAsync_ShouldReturnCheepsOrderedByTimestampDescendingFromSpesificAuthor()
    {
        // Arrange
        var author1 = new Author { AuthorId = 1, Name = "TestUser1", Email = "test1@test.com" };
        var author2 = new Author { AuthorId = 2, Name = "TestUser2", Email = "test2@test.com" };
        var authors = new List<Author> { author1, author2 };
        var cheeps = new List<Cheep>
        {
            new() { CheepId = 6, AuthorId = 1, Author = author1, Text = "Cheep 6", TimeStamp = DateTime.Now },
            new() { CheepId = 5, AuthorId = 1, Author = author1, Text = "Cheep 5", TimeStamp = DateTime.Now.AddHours(-1) },
            new() { CheepId = 4, AuthorId = 1, Author = author1, Text = "Cheep 4", TimeStamp = DateTime.Now.AddHours(-2) },
            new() { CheepId = 3, AuthorId = 2, Author = author2, Text = "Cheep 3", TimeStamp = DateTime.Now.AddHours(-3) },
            new() { CheepId = 2, AuthorId = 2, Author = author2, Text = "Cheep 2", TimeStamp = DateTime.Now.AddHours(-4) },
            new() { CheepId = 1, AuthorId = 2, Author = author2, Text = "Cheep 1", TimeStamp = DateTime.Now.AddHours(-5) }
        };

        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        var mockAuthorSet = authors.BuildMockDbSet();

        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);

        var repository = new CheepRepository(mockContext.Object);

        Author? result = await repository.ReadPageFromAuthorAsync("TestUser2");

        result.Should().NotBeNull();
        result!.Name.Should().Be("TestUser2");
        result!.Email.Should().Be("test2@test.com");
        result.Cheeps.Should().HaveCount(3);
        result.Cheeps.First().Text.Should().Be("Cheep 3"); // shows the newst cheep first
        result.Cheeps.Last().Text.Should().Be("Cheep 1");  // shows the oldest cheep last
    }

    [Fact]
    public async Task PostCheepAsync_ShouldReturnCheepsOrderedByTimestampDescendingFromSpesificAuthor()
    {
        // Arrange
        var author1 = new Author { AuthorId = 1, Name = "TestUser1", Email = "test1@test.com" };
        var authors = new List<Author> { author1 };

        var cheeps = new List<Cheep>
        {
            new() { CheepId = 1, AuthorId = 1, Author = author1, Text = "Cheep 1", TimeStamp = DateTime.Now.AddHours(-1) },
        };

        var cheep = new Cheep { CheepId = 2, AuthorId = 1, Author = author1, Text = "Cheep 2", TimeStamp = DateTime.Now };


        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        var mockAuthorSet = authors.BuildMockDbSet();

        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockCheepSet.Setup(d => d.Add(It.IsAny<Cheep>())).Callback<Cheep>(c => cheeps.Add(c));

        var repository = new CheepRepository(mockContext.Object);

        await repository.PostAsync(cheep);
        var result = await repository.ReadPageAsync(0);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Text.Should().Be("Cheep 2");
        result.Last().Text.Should().Be("Cheep 1");
    }

    [Fact]
    public async Task PostAuthorAsync_ShouldReturnCheepsOrderedByTimestampDescendingFromSpesificAuthor()
    {
        // Arrange
        var cheeps = new List<Cheep> { };
        var authors = new List<Author> { };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockAuthorSet.Setup(d => d.Add(It.IsAny<Author>())).Callback<Author>(c => authors.Add(c));

        var repository = new CheepRepository(mockContext.Object);

        var author = new Author { AuthorId = 1, Name = "TestUser", Email = "test@test.com" };

        repository.InsertAuthor(author);

        Author? result = await repository.ReadPageFromAuthorAsync("TestUser");

        result.Should().NotBeNull();
        result!.Name.Should().Be("TestUser");
        result!.Email.Should().Be("test@test.com");
    }

    [Fact]
    public void GetAuthorFromUsername_test()
    {
        // Arrange
        var cheeps = new List<Cheep> { };

        var author1 = new Author { AuthorId = 1, Name = "TestUser1", Email = "test1@test.com" };
        var author2 = new Author { AuthorId = 2, Name = "TestUser2", Email = "test2@test.com" };
        var authors = new List<Author> { author1, author2 };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockAuthorSet.Setup(d => d.Add(It.IsAny<Author>())).Callback<Author>(c => authors.Add(c));

        var repository = new CheepRepository(mockContext.Object);

        Author? result = repository.GetAuthorFromUsername("TestUser1");

        result.Should().NotBeNull();
        result!.Name.Should().Be("TestUser1");
        result!.Email.Should().Be("test1@test.com");
    }

    [Fact]
    public void GetAuthorFromAuthorID_test()
    {
        // Arrange
        var cheeps = new List<Cheep> { };

        var author1 = new Author { AuthorId = 1, Name = "TestUser1", Email = "test1@test.com" };
        var author2 = new Author { AuthorId = 2, Name = "TestUser2", Email = "test2@test.com" };
        var authors = new List<Author> { author1, author2 };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockAuthorSet.Setup(d => d.Add(It.IsAny<Author>())).Callback<Author>(c => authors.Add(c));

        var repository = new CheepRepository(mockContext.Object);

        Author? result = repository.GetAuthorFromAuthorID(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("TestUser1");
        result!.Email.Should().Be("test1@test.com");
    }
}
