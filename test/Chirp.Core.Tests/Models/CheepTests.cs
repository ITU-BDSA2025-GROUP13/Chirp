using Chirp.Core.Models;

namespace Chirp.Core.Tests.Models;

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

        Assert.Equal(1, cheep.CheepId);
        Assert.Equal(text, cheep.Text);
        Assert.Equal(author, cheep.Author);
        Assert.Equal(0, cheep.AuthorId);
        Assert.Equal(timestamp, cheep.TimeStamp);
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

        Assert.Equal(text, cheep.Text);
    }
}
