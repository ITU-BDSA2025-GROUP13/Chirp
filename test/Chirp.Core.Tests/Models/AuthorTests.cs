using Chirp.Core.Models;

namespace Chirp.Core.Tests.Models;

public class AuthorTests
{
    [Fact]
    public void AuthorConstructor_WhenInvoked_InitializesPropertiesWithDefaults()
    {
        var author = new Author();

        Assert.Equal(0, author.AuthorId);
        Assert.Equal(string.Empty, author.Name);
        Assert.Equal(string.Empty, author.Email);
        Assert.NotNull(author.Cheeps);
        Assert.Empty(author.Cheeps);
    }

    [Fact]
    public void Properties_WhenSet_HoldAssignedValues()
    {
        string name = "TestUser";
        string email = "test@example.com";

        var author = new Author();

        author.Name = name;
        author.Email = email;

        Assert.Equal(name, author.Name);
        Assert.Equal(email, author.Email);
    }

    [Fact]
    public void Cheeps_WhenItemIsAdded_ContainsTheItem()
    {
        var author = new Author();
        var cheep = new Cheep { TimeStamp = DateTime.Now, AuthorId = 0, Author = author, Text = "hello" };

        author.Cheeps.Add(cheep);

        Assert.Single(author.Cheeps);
        Assert.Contains(cheep, author.Cheeps);
    }
}
