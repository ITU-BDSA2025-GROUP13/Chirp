using Chirp.Core.Models;

using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Tests.Repositories;

public class AuthorRepositoryTests
{
    private IChirpDbContext CreateDatabase()
    {
        string dbPath = $"{Path.GetTempPath()}/chirp/authorRepoTest.db";
        Environment.SetEnvironmentVariable("DB_PATH", dbPath);

        DbContextOptions<ChirpDbContext> options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseInMemoryDatabase(databaseName: $"{dbPath}/{Guid.NewGuid()}")
            .Options;

        return new ChirpDbContext(options);
    }

    [Fact]
    public void InsertAuthor_AddsAuthorToDB()
    {
        IChirpDbContext context = CreateDatabase();
        IAuthorRepository authorRepo = new AuthorRepository(context);
        authorRepo.InsertAuthor(new Author());
        context.SaveChanges();

        Assert.Single(authorRepo.GetAllAuthors());
    }

    [Fact]
    public void GetAuthorFromName_ReturnsAuthor()
    {
        IChirpDbContext context = CreateDatabase();
        IAuthorRepository authorRepo = new AuthorRepository(context);
        Author author = new Author { Name = "Karl 'DROP TABLE user;' Fortnite the -3." };
        authorRepo.InsertAuthor(author);
        context.SaveChanges();
        var result = authorRepo.GetAuthorByName(author.Name);
        Assert.Equivalent(author, result);
    }

    [Fact]
    public void GetAuthorFromEmail_ReturnsAuthor()
    {
        IChirpDbContext context = CreateDatabase();
        IAuthorRepository authorRepo = new AuthorRepository(context);
        Author author = new Author { Email = "Karl'DROP TABLE user;'Fortnite@the-3." };
        authorRepo.InsertAuthor(author);
        context.SaveChanges();
        var result = authorRepo.GetAuthorByEmail(author.Email);
        Assert.Equivalent(author, result);
    }

    [Fact]
    public void GetAuthorFromID_ReturnsAuthor()
    {
        IChirpDbContext context = CreateDatabase();
        IAuthorRepository authorRepo = new AuthorRepository(context);
        Author author = new Author { AuthorId = 1 };
        authorRepo.InsertAuthor(author);
        context.SaveChanges();
        var result = authorRepo.GetAuthorByID(author.AuthorId);
        Assert.Equivalent(author, result);
    }
}