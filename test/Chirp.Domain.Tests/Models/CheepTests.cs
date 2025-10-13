using FluentAssertions;
using Chirp.Domain;
using Xunit;

namespace Chirp.Domain.Tests.Models;

public class CheepTests
{
    [Fact]
    public void Cheep_WhenPropertiesAreSet_HoldsCorrectValues()
    {
        string text = "hello";
        var author = new Author { Name = "TestUser" };
        var timestamp = DateTime.Now;

        var cheep = new Cheep
        {
            CheepId = 1,
            Text = text,
            Author = author,
            AuthorId = 0,
            TimeStamp = timestamp
        };

        cheep.CheepId.Should().Be(1);
        cheep.Text.Should().Be(text);
        cheep.Author.Should().Be(author);
        cheep.AuthorId.Should().Be(0);
        cheep.TimeStamp.Should().Be(timestamp);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Hello world")]
    [InlineData("Very long message with lots of text")]
    public void CheepText_WhenSet_AcceptsVariousStringValues(string text)
    {
        var author = new Author { Name = "TestUser" };
        var timestamp = DateTime.Now;

        var cheep = new Cheep
        {
            CheepId = 1,
            Text = text,
            Author = author,
            AuthorId = 0,
            TimeStamp = timestamp
        };

        cheep.Text.Should().Be(text);
    }
}
