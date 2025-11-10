using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Tests.DatabaseContext;

public class ChirpDbContextsTests
{
    [Fact]
    public async Task SaveChangesAsync_WhenAddingAuthorAndCheep_SavesThemToDatabase()
    {
        string name = "Karl";
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ChirpDbContext(options);

        var author = new ChirpUser { UserName = name, Email = "test@test.com" };
        await context.SaveChangesAsync();

        var cheep = new Cheep
        {
            Author = author,
            AuthorId = author.Id,
            Text = "Hello World",
            TimeStamp = DateTime.Now
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        var savedCheep = await context.Cheeps
            .Include(c => c.Author)
            .FirstOrDefaultAsync();
        Assert.NotNull(savedCheep);
        Assert.NotNull(savedCheep.Author);
        Assert.Equal(name, savedCheep.Author.UserName);
    }

    [Fact]
    public async Task Cheeps_WhenQueriedWithIncludeAuthor_ReturnsCheepsWithAuthorData()
    {
        string name = "Alice";
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ChirpDbContext(options);
        var author = new ChirpUser { UserName = name, Email = "alice@test.com" };
        await context.SaveChangesAsync();

        var cheep = new Cheep
        {
            Author = author,
            AuthorId = author.Id,
            Text = "Test message",
            TimeStamp = DateTime.Now
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        var result = await context.Cheeps
            .Include(c => c.Author)
            .ToListAsync();

        Assert.Single(result);
        Assert.Equal(name, result.First().Author.UserName);
    }
}
