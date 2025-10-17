using Chirp.Domain;
using FluentAssertions;
using Xunit;

namespace Chirp.Domain.Tests.Models;

public class AuthorTests
{
    [Fact]
    public void Author_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var author = new Author();

        // Assert
        author.AuthorId.Should().Be(0);
        author.Name.Should().Be(string.Empty);
        author.Email.Should().Be(string.Empty);
        author.PasswordHash.Should().Be(string.Empty);
        author.Cheeps.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Author_Properties_ShouldBeSettable()
    {
        // Arrange
        var author = new Author();

        // Act
        author.Name = "TestUser";
        author.Email = "test@example.com";
        author.PasswordHash = "hashedpassword123";

        // Assert
        author.Name.Should().Be("TestUser");
        author.Email.Should().Be("test@example.com");
        author.PasswordHash.Should().Be("hashedpassword123");
    }

    [Fact]
    public void Author_Cheeps_ShouldBeModifiable()
    {
        // Arrange
        var author = new Author();
        var cheep = new Cheep { TimeStamp = DateTime.Now, AuthorId = 0, Author = author, Text = "hello" };

        // Act
        author.Cheeps.Add(cheep);

        // Assert
        author.Cheeps.Should().HaveCount(1);
        author.Cheeps.Should().Contain(cheep);
    }
}
