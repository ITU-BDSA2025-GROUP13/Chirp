using Chirp.Infrastructure;
using FluentAssertions;
using Chirp.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Tests.Repository;

public class ChirpDbContextsTests
{
    [Fact]
    public async Task CanCreateContext_And_AddData()
    {
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ChirpDbContext(options);

        var author = new Author { Name = "Test User", Email = "test@test.com" };
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
        savedCheep.Author!.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task CanQueryCheepsWithAuthors()
    {
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new ChirpDbContext(options);
        var author = new Author { Name = "Alice", Email = "alice@test.com" };
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
        result.First().Author!.Name.Should().Be("Alice");
    }
}
