using Chirp.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Tests.Repository;

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

        var author = new Author { Name = name, Email = "test@test.com" };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        var cheep = new Cheep
        {
            Author = author,
            AuthorId = author.AuthorId,
            Text = "Hello World",
            TimeStamp = DateTime.Now
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        var savedCheep = await context.Cheeps
            .Include(c => c.Author)
            .FirstOrDefaultAsync();
        savedCheep.Should().NotBeNull();
        savedCheep!.Author.Should().NotBeNull();
        savedCheep.Author!.Name.Should().Be(name);
    }

    [Fact]
    public async Task Cheeps_WhenQueriedWithIncludeAuthor_ReturnsCheepsWithAuthorData()
    {
        string name = "Alice";
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ChirpDbContext(options);
        var author = new Author { Name = name, Email = "alice@test.com" };
        context.Authors.Add(author);
        await context.SaveChangesAsync();

        var cheep = new Cheep
        {
            Author = author,
            AuthorId = author.AuthorId,
            Text = "Test message",
            TimeStamp = DateTime.Now
        };
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();

        var result = await context.Cheeps
            .Include(c => c.Author)
            .ToListAsync();

        result.Should().HaveCount(1);
        result.First().Author!.Name.Should().Be(name);
    }
}
