using FluentAssertions;
using Chirp.Domain;
using Xunit;

namespace Chirp.Domain.Tests.Models;

public class CheepTests
{
    [Fact]
    public void Cheep_Properties_ShouldBeSettable()
    {
        // Arrange
        var author = new Author { Name = "TestUser" };
        var timestamp = DateTime.Now;

        // Act
        var cheep = new Cheep
        {
            CheepId = 1,
            Text = "hello",
            Author = author,
            AuthorId = 0,
            TimeStamp = timestamp
        };

        // Assert
        cheep.CheepId.Should().Be(1);
        cheep.Text.Should().Be("hello");
        cheep.Author.Should().Be(author);
        cheep.AuthorId.Should().Be(0);
        cheep.TimeStamp.Should().Be(timestamp);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Hello world")]
    [InlineData("Very long message with lots of text")]
    public void Cheep_Text_ShouldAcceptDifferentValues(string text)
    {
        var author = new Author { Name = "TestUser" };
        var timestamp = DateTime.Now;

        // Act
        var cheep = new Cheep
        {
            CheepId = 1,
            Text = text,
            Author = author,
            AuthorId = 0,
            TimeStamp = timestamp
        };

        // Assert
        cheep.Text.Should().Be(text);
    }
}
