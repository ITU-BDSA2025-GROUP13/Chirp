using Chirp.Core.Models;
using FluentAssertions;

namespace Chirp.Core.Tests.Models;

public class AuthorTests
{
    [Fact]
    public void AuthorConstructor_WhenInvoked_InitializesPropertiesWithDefaults()
    {
        var author = new Author();

        author.AuthorId.Should().Be(0);
        author.Name.Should().Be(string.Empty);
        author.Email.Should().Be(string.Empty);
        author.Cheeps.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Properties_WhenSet_HoldAssignedValues()
    {
        string name = "TestUser";
        string email = "test@example.com";

        var author = new Author();

        author.Name = name;
        author.Email = email;

        author.Name.Should().Be(name);
        author.Email.Should().Be(email);
    }

    [Fact]
    public void Cheeps_WhenItemIsAdded_ContainsTheItem()
    {
        var author = new Author();
        var cheep = new Cheep { TimeStamp = DateTime.Now, AuthorId = 0, Author = author, Text = "hello" };

        author.Cheeps.Add(cheep);

        author.Cheeps.Should().HaveCount(1);
        author.Cheeps.Should().Contain(cheep);
    }
}
