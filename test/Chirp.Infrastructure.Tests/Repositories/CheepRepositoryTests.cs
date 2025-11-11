using Chirp.Core.Models;

using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;

namespace Chirp.Infrastructure.Tests.Repositories;

public class CheepRepositoryTests
{
    [Fact]
    public async Task ReadPageAsync_WhenCalled_ReturnsCheepsInDescendingOrder()
    {
        string text3 = "Newest cheep";
        string text2 = "Older cheep";
        string text1 = "Oldest cheep";
        var author = new ChirpUser { Id = "1", UserName = "TestUser", Email = "test@test.com" };
        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 3, AuthorId = "1", Author = author, Text = text3, TimeStamp = DateTime.Now },
            new Cheep { CheepId = 2, AuthorId = "1", Author = author, Text = text2, TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep { CheepId = 1, AuthorId = "1", Author = author, Text = text1, TimeStamp = DateTime.Now.AddHours(-2) }
        };

        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        var cheepRepo = new CheepRepository(mockContext.Object);

        var result = await cheepRepo.GetMainPage(0);

        Assert.Equal(cheeps.Count(), result.Count());
        Assert.Equal(text3, result.First().Text);
        Assert.Equal(text1, result.Last().Text);
    }

    [Fact]
    public async Task PostAsync_WhenCheepIsAdded_ItAppearsFirstInList()
    {
        string text2 = "Cheep 2";
        string text1 = "Cheep 1";

        string name1 = "TestUser1";

        var author1 = new ChirpUser { Id = "1", UserName = name1, Email = "test1@test.com" };
        var authors = new List<ChirpUser> { author1 };

        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 1, AuthorId = "1", Author = author1, Text = text1, TimeStamp = DateTime.Now.AddHours(-1) },
        };

        var cheep = new Cheep { CheepId = 2, AuthorId = "1", Author = author1, Text = text2, TimeStamp = DateTime.Now };


        var mockContext = new Mock<IChirpDbContext>();

        var mockCheepSet = cheeps.BuildMockDbSet();
        var mockAuthorSet = authors.BuildMockDbSet();

        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockCheepSet.Setup(d => d.Add(It.IsAny<Cheep>())).Callback<Cheep>(c => cheeps.Add(c));

        var repository = new CheepRepository(mockContext.Object);

        await repository.InsertCheep(cheep);
        var result = await repository.GetMainPage(0);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(text2, result.First().Text);
        Assert.Equal(text1, result.Last().Text);
    }

    [Fact]
    public void CheepOver160Chars_ReturnsError()
    {
        string authorID = "1";
        ChirpUser author1 = new ChirpUser { Id = authorID, UserName = "Karl Fortnite", Email = "KarlFortnite@gmail.com" };
        Cheep cheep = new Cheep
        {
            CheepId = 1,
            AuthorId = authorID,
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
