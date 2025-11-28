using Chirp.Core.Models;

using Microsoft.AspNetCore.Identity;
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

    [Fact]
    public async Task GetCheepByID_ReturnsCorrectCheep()
    {
        var author = new ChirpUser { Id = "0", UserName = "TestUser", Email = "test@test.com" };
        var cheepInQuestion = new Cheep { CheepId = 0, AuthorId = author.Id, Author = author, Text = "text", TimeStamp = DateTime.Now.AddHours(-4) };
        var cheeps = new List<Cheep>
        {
            new Cheep { CheepId = 3, AuthorId = "1", Author = author, Text = "text3", TimeStamp = DateTime.Now.AddHours(-1) },
            new Cheep { CheepId = 2, AuthorId = "1", Author = author, Text = "text2", TimeStamp = DateTime.Now.AddHours(-2) },
            new Cheep { CheepId = 1, AuthorId = "1", Author = author, Text = "text1", TimeStamp = DateTime.Now.AddHours(-3) },
            cheepInQuestion
        };

        var mockContext = new Mock<IChirpDbContext>();
        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);

        var repository = new CheepRepository(mockContext.Object);

        Cheep? result = await repository.GetCheepById(0);
        Assert.Equivalent(result, cheepInQuestion);
    }

    [Fact]
    public async Task DeleteCheep_WhenCalled_RemovesCheep()
    {
        string text = "Cheep to be deleted";
        var author = new ChirpUser { Id = "0", UserName = "TestUser", Email = "test@test.com" };
        var cheep = new Cheep { CheepId = 0, AuthorId = author.Id, Author = author, Text = text, TimeStamp = DateTime.Now };
        var cheeps = new List<Cheep> { cheep };

        var mockContext = new Mock<IChirpDbContext>();
        var mockCheepSet = cheeps.BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);
        mockCheepSet.Setup(d => d.Remove(It.IsAny<Cheep>())).Callback<Cheep>(c => cheeps.Remove(c));

        var repository = new CheepRepository(mockContext.Object);

        Cheep? resultBeforeDeletion = await repository.GetCheepById(0);
        Assert.Equivalent(resultBeforeDeletion, cheep);

        await repository.DeleteCheep(cheep);

        Cheep? resultAfterDeletion = await repository.GetCheepById(0);
        Assert.Null(resultAfterDeletion);
    }

    [Fact]
    public async Task EditCheep_WhenCalledCorrectly_ModifiesCheep()
    {
        string originalText = "Original message";
        string editedText = "Edited message";

        var author = new ChirpUser { Id = "1", UserName = "TestUser", Email = "test@test.com" };
        var cheep = new Cheep { CheepId = 1, AuthorId = "1", Author = author, Text = originalText, TimeStamp = DateTime.Now.AddHours(-2) };
        var authors = new List<ChirpUser> { author };
        var cheeps = new List<Cheep> { cheep };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();

        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);

        var cheepRepo = new CheepRepository(mockContext.Object);

        await cheepRepo.EditCheepById(cheep.CheepId, editedText);

        var result = await cheepRepo.GetMainPage(0);

        Assert.Equal(result.First().Text, editedText);
    }

    [Fact]
    public void EditCheep_WhenCalledWithNonexistentCheepId_Throws()
    {
        string originalText = "Original message";
        string editedText = "Edited message";
        int existingCheepId = 1;
        int nonexistentCheepId = 2;

        var author = new ChirpUser { Id = "1", UserName = "TestUser", Email = "test@test.com" };
        var cheep = new Cheep { CheepId = existingCheepId, AuthorId = "1", Author = author, Text = originalText, TimeStamp = DateTime.Now.AddHours(-2) };
        var authors = new List<ChirpUser> { author };
        var cheeps = new List<Cheep> { cheep };

        var mockContext = new Mock<IChirpDbContext>();

        var mockAuthorSet = authors.BuildMockDbSet();

        var mockCheepSet = cheeps.BuildMockDbSet();

        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);

        var cheepRepo = new CheepRepository(mockContext.Object);

        Assert.Throws<DbUpdateException>(cheepRepo.EditCheepById(nonexistentCheepId, editedText).GetAwaiter().GetResult);
    }

    [Fact]
    public async Task InsertCheep_WhenAuthorIdNull_Throws()
    {
        var author = new ChirpUser { Id = "author", UserName = "Author", Email = "author@test.com" };
        var cheep = new Cheep
        {
            CheepId = 1,
            AuthorId = null!,
            Author = author,
            Text = "Invalid",
            TimeStamp = DateTime.UtcNow
        };

        var mockContext = new Mock<IChirpDbContext>();
        var mockCheepSet = new List<Cheep>().BuildMockDbSet();
        mockContext.Setup(c => c.Cheeps).Returns(mockCheepSet.Object);

        var repository = new CheepRepository(mockContext.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.InsertCheep(cheep));
    }

    [Fact]
    public async Task GetPrivateTimelineCheeps_ReturnsOwnAndFollowedCheepsWithReplies()
    {
        await using ChirpDbContext context = CreateInMemoryContext();
        var repository = new CheepRepository(context);

        var owner = new ChirpUser { Id = "owner", UserName = "Owner", Email = "owner@test.com" };
        var followed = new ChirpUser { Id = "followed", UserName = "Followed", Email = "followed@test.com" };
        var outsider = new ChirpUser { Id = "outsider", UserName = "Outsider", Email = "outsider@test.com" };

        context.ChirpUsers.AddRange(owner, followed, outsider);
        owner.FollowsList.Add(followed);
        await context.SaveChangesAsync();

        DateTime baseTime = DateTime.UtcNow;

        var ownerCheep = new Cheep
        {
            CheepId = 1,
            AuthorId = owner.Id,
            Author = owner,
            Text = "Owner root",
            TimeStamp = baseTime.AddMinutes(-1)
        };

        var followedCheep = new Cheep
        {
            CheepId = 2,
            AuthorId = followed.Id,
            Author = followed,
            Text = "Followed root",
            TimeStamp = baseTime
        };

        var outsiderCheep = new Cheep
        {
            CheepId = 4,
            AuthorId = outsider.Id,
            Author = outsider,
            Text = "Outsider root",
            TimeStamp = baseTime.AddMinutes(-2)
        };

        var reply = new Cheep
        {
            CheepId = 3,
            AuthorId = owner.Id,
            Author = owner,
            Text = "Reply",
            TimeStamp = baseTime.AddSeconds(-30),
            ParentCheep = followedCheep
        };

        context.Cheeps.AddRange(ownerCheep, followedCheep, outsiderCheep, reply);
        await context.SaveChangesAsync();

        ChirpUser trackedOwner = await context.ChirpUsers.FirstAsync(u => u.Id == owner.Id);

        List<Cheep> result = (await repository.GetPrivateTimelineCheeps(trackedOwner)).ToList();

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, c => c.AuthorId == outsider.Id);
        Cheep followedFromResult = Assert.Single(result, c => c.CheepId == followedCheep.CheepId);
        Assert.Single(followedFromResult.Replies);
        Assert.Equal(reply.CheepId, followedFromResult.Replies.First().CheepId);
        Assert.True(result[0].TimeStamp >= result[1].TimeStamp);
    }

    [Fact]
    public async Task GetListOfFollowers_WhenCalled_LoadsFollowList()
    {
        await using ChirpDbContext context = CreateInMemoryContext();
        var repository = new ChirpUserRepository(context);

        var owner = new ChirpUser { Id = "owner", UserName = "Owner", Email = "owner@test.com" };
        var follower = new ChirpUser { Id = "follower", UserName = "Follower", Email = "follower@test.com" };

        context.ChirpUsers.AddRange(owner, follower);
        owner.FollowsList.Add(follower);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        ChirpUser trackedOwner = await context.ChirpUsers.FirstAsync(u => u.Id == owner.Id);
        Assert.False(context.Entry(trackedOwner).Collection(u => u.FollowsList).IsLoaded);

        List<ChirpUser> result = await repository.GetListOfFollowers(trackedOwner);

        Assert.True(context.Entry(trackedOwner).Collection(u => u.FollowsList).IsLoaded);
        var loadedFollower = Assert.Single(result);
        Assert.Equal(follower.Id, loadedFollower.Id);
    }

    [Fact]
    public async Task GetAuthorPage_ReturnsCheepsWithReplies()
    {
        await using ChirpDbContext context = CreateInMemoryContext();
        var repository = new CheepRepository(context);

        var author = new ChirpUser { Id = "author", UserName = "Author", Email = "author@test.com" };
        var other = new ChirpUser { Id = "other", UserName = "Other", Email = "other@test.com" };
        context.ChirpUsers.AddRange(author, other);
        await context.SaveChangesAsync();

        DateTime baseTime = DateTime.UtcNow;

        var firstCheep = new Cheep
        {
            CheepId = 1,
            AuthorId = author.Id,
            Author = author,
            Text = "First",
            TimeStamp = baseTime.AddMinutes(-5)
        };

        var latestCheep = new Cheep
        {
            CheepId = 2,
            AuthorId = author.Id,
            Author = author,
            Text = "Latest",
            TimeStamp = baseTime
        };

        var reply = new Cheep
        {
            CheepId = 3,
            AuthorId = author.Id,
            Author = author,
            Text = "Reply",
            TimeStamp = baseTime.AddMinutes(-1),
            ParentCheep = latestCheep
        };

        var otherCheep = new Cheep
        {
            CheepId = 4,
            AuthorId = other.Id,
            Author = other,
            Text = "Other",
            TimeStamp = baseTime.AddMinutes(-2)
        };

        context.Cheeps.AddRange(firstCheep, latestCheep, reply, otherCheep);
        await context.SaveChangesAsync();

        List<Cheep> result = (await repository.GetAuthorPage(author)).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(author.Id, c.AuthorId));
        Assert.DoesNotContain(result, c => c.CheepId == otherCheep.CheepId);
        Cheep latestFromResult = Assert.Single(result, c => c.CheepId == latestCheep.CheepId);
        Assert.Single(latestFromResult.Replies);
        Assert.Equal(reply.CheepId, latestFromResult.Replies[0].CheepId);
        Assert.True(result[0].TimeStamp >= result[1].TimeStamp);
    }

    [Fact]
    public async Task GetAllAuthorCheeps_ReturnsAllCheepsOrdered()
    {
        await using ChirpDbContext context = CreateInMemoryContext();
        var repository = new CheepRepository(context);

        var author = new ChirpUser { Id = "author", UserName = "Author", Email = "author@test.com" };
        var other = new ChirpUser { Id = "other", UserName = "Other", Email = "other@test.com" };
        context.ChirpUsers.AddRange(author, other);

        DateTime baseTime = DateTime.UtcNow;

        var oldest = new Cheep
        {
            CheepId = 1,
            AuthorId = author.Id,
            Author = author,
            Text = "Old",
            TimeStamp = baseTime.AddMinutes(-10)
        };

        var middle = new Cheep
        {
            CheepId = 2,
            AuthorId = author.Id,
            Author = author,
            Text = "Middle",
            TimeStamp = baseTime.AddMinutes(-5)
        };

        var newest = new Cheep
        {
            CheepId = 3,
            AuthorId = author.Id,
            Author = author,
            Text = "New",
            TimeStamp = baseTime
        };

        var otherCheep = new Cheep
        {
            CheepId = 4,
            AuthorId = other.Id,
            Author = other,
            Text = "Other",
            TimeStamp = baseTime.AddMinutes(-1)
        };

        context.Cheeps.AddRange(oldest, middle, newest, otherCheep);
        await context.SaveChangesAsync();

        List<Cheep> result = (await repository.GetAllAuthorCheeps(author)).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, c => Assert.Equal(author.Id, c.AuthorId));
        Assert.True(result.SequenceEqual(result.OrderByDescending(c => c.TimeStamp)));
        Assert.DoesNotContain(result, c => c.AuthorId == other.Id);
    }

    private static ChirpDbContext CreateInMemoryContext()
    {
        DbContextOptions<ChirpDbContext> options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ChirpDbContext(options);
    }
}
