using Chirp.Core.Models;
using FluentAssertions;
using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using Xunit.Sdk;

namespace Chirp.Infrastructure.Tests.Repositories;

public class CheepRepositoryTests
{
    [Fact]
    public async Task ReadPageAsync_WhenCalled_ReturnsCheepsInDescendingOrder()
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

        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        var cheepRepo = new CheepRepository(mockContext.Object);

        var result = await cheepRepo.GetMainPage(0);

        result.Should().HaveCount(cheeps.Count());
        result.First().Text.Should().Be(text3);
        result.Last().Text.Should().Be(text1);
    }

    [Fact]
    public async Task ReadPageFromAuthorAsync_WithValidAuthorName_ReturnsAuthorWithCheepsInDescendingOrder()
    {
        string text6 = "Cheep 6";
        string text5 = "Cheep 5";
        string text4 = "Cheep 4";
        string text3 = "Cheep 3";
        string text2 = "Cheep 2";
        string text1 = "Cheep 1";

        string name2 = "TestUser2";
        string name1 = "TestUser1";

        string email2 = "test2@test.com";
        string email1 = "test1@test.com";

        var author1 = new Author { AuthorId = 1, Name = name1, Email = email1 };
        var author2 = new Author { AuthorId = 2, Name = name2, Email = email2 };
        var authors = new List<Author> { author1, author2 };
        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 6, AuthorId = 1, Author = author1, Text = text6, TimeStamp = DateTime.Now },
            new Cheep { CheepId = 5, AuthorId = 1, Author = author1, Text = text5, TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep { CheepId = 4, AuthorId = 1, Author = author1, Text = text4, TimeStamp = DateTime.Now.AddHours(-2) },
            new Cheep { CheepId = 3, AuthorId = 2, Author = author2, Text = text3, TimeStamp = DateTime.Now.AddHours(-3) },
            new Cheep { CheepId = 2, AuthorId = 2, Author = author2, Text = text2, TimeStamp = DateTime.Now.AddHours(-4) },
            new Cheep { CheepId = 1, AuthorId = 2, Author = author2, Text = text1, TimeStamp = DateTime.Now.AddHours(-5) }
        };
        author1.Cheeps = cheeps.Where(c => c.AuthorId == author1.AuthorId).ToList();
        author2.Cheeps = cheeps.Where(c => c.AuthorId == author2.AuthorId).ToList();

        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        var mockAuthorSet = authors.BuildMockDbSet();

        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);

        var cheepRepo = new CheepRepository(mockContext.Object);
        var authorRepo = new AuthorRepository(mockContext.Object);

        Author? result = authorRepo.GetAuthorByName("TestUser2");

        result.Should().NotBeNull();
        result!.Name.Should().Be(name2);
        result!.Email.Should().Be(email2);
        result.Cheeps.Should().HaveCount(3);
        result.Cheeps.First().Text.Should().Be(text3); // shows the newst cheep first
        result.Cheeps.Last().Text.Should().Be(text1);  // shows the oldest cheep last
    }

    [Fact]
    public async Task PostAsync_WhenCheepIsAdded_ItAppearsFirstInList()
    {
        string text2 = "Cheep 2";
        string text1 = "Cheep 1";

        string name1 = "TestUser1";

        var author1 = new Author { AuthorId = 1, Name = name1, Email = "test1@test.com" };
        var authors = new List<Author> { author1 };

        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 1, AuthorId = 1, Author = author1, Text = text1, TimeStamp = DateTime.Now.AddHours(-1) },
        };

        var cheep = new Cheep { CheepId = 2, AuthorId = 1, Author = author1, Text = text2, TimeStamp = DateTime.Now };


        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        var mockAuthorSet = authors.BuildMockDbSet();

        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockCheepSet.Setup(d => d.Add(It.IsAny<Cheep>())).Callback<Cheep>(c => cheeps.Add(c));

        var repository = new CheepRepository(mockContext.Object);

        await repository.InsertCheep(cheep);
        var result = await repository.GetMainPage(0);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Text.Should().Be(text2);
        result.Last().Text.Should().Be(text1);
    }

    [Fact]
    public async Task InsertAuthor_WhenNewAuthorIsAdded_AuthorCanBeRetrieved()
    {
        var cheeps = new List<Cheep> { };
        var authors = new List<Author> { };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockAuthorSet.Setup(d => d.Add(It.IsAny<Author>())).Callback<Author>(c => authors.Add(c));

        var cheepRepo = new CheepRepository(mockContext.Object);
        var authorRepo = new AuthorRepository(mockContext.Object);

        var author = new Author { AuthorId = 1, Name = "TestUser", Email = "test@test.com" };

        authorRepo.InsertAuthor(author);

        Author? result = authorRepo.GetAuthorByName("TestUser");

        result.Should().NotBeNull();
        result!.Name.Should().Be("TestUser");
        result!.Email.Should().Be("test@test.com");
    }

    [Fact]
    public void GetAuthorFromUsername_WithValidUsername_ReturnsCorrectAuthor()
    {
        string name1 = "TestUser1";
        string email1 = "test1@test.com";

        var cheeps = new List<Cheep> { };

        var author1 = new Author { AuthorId = 1, Name = name1, Email = email1 };
        var author2 = new Author { AuthorId = 2, Name = "TestUser2", Email = "test2@test.com" };
        var authors = new List<Author> { author1, author2 };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockAuthorSet.Setup(d => d.Add(It.IsAny<Author>())).Callback<Author>(c => authors.Add(c));

        var repository = new AuthorRepository(mockContext.Object);

        Author? result = repository.GetAuthorByName("TestUser1");

        result.Should().NotBeNull();
        result!.Name.Should().Be(name1);
        result!.Email.Should().Be(email1);
    }

    [Fact]
    public void GetAuthorFromAuthorID_WithValidAuthorID_ReturnsCorrectAuthor()
    {
        string name1 = "TestUser1";
        string email1 = "test1@test.com";

        var cheeps = new List<Cheep> { };

        var author1 = new Author { AuthorId = 1, Name = name1, Email = email1 };
        var author2 = new Author { AuthorId = 2, Name = "TestUser2", Email = "test2@test.com" };
        var authors = new List<Author> { author1, author2 };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockContext.Setup(c => c.Authors).Returns(mockAuthorSet.Object);
        mockAuthorSet.Setup(d => d.Add(It.IsAny<Author>())).Callback<Author>(c => authors.Add(c));

        var repository = new AuthorRepository(mockContext.Object);

        Author? result = repository.GetAuthorByID(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be(name1);
        result!.Email.Should().Be(email1);
    }

    [Fact]
    public void CheepOver160Chars_ReturnsError()
    {
        int authorID = 1;
        Author author1 = new Author { AuthorId = authorID, Name = "Karl Fortnite", Email = "KarlFortnite@gmail.com" };
        Cheep cheep = new Cheep
        {
            CheepId = 1,
            AuthorId = 1,
            Author = author1,
            TimeStamp = DateTime.Now,
            Text = "OOne morning, when Gregor Samsa woke from troubled dreams, he found himself transformed in his bed into a horrible vermin. He lay on his armour-like back, and if he lifted his head."
        };

        string dbPath = $"{Path.GetTempPath()}/chirp/cheepDBtest.db";
        Environment.SetEnvironmentVariable("DB_PATH", dbPath);

        DbContextOptions<ChirpDbContext> options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite("Datasource=:memory:")
            .Options;

        IChirpDbContext context = new ChirpDbContext(options);
        ICheepRepository cheepRepository = new CheepRepository(context);
        Assert.Throws<DbUpdateException>(cheepRepository.InsertCheep(cheep).GetAwaiter().GetResult);
    }
}
