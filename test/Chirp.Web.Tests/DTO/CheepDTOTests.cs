using Chirp.Infrastructure.Services;

namespace Chirp.Web.Tests.DTO;

public class CheepDTOTests
{
    [Fact]
    public void CheepDTOTest()
    {
        var cheepDto = new CheepDTO(
            Text: "Test message",
            DatePosted: "2023-01-01 12:00:00",
            AuthorName: "TestAuthor",
            CheepId: 0,
            ParentCheepID: 1,
            Replies: new List<CheepDTO>(),
            Likes: "0"
        );

        Assert.Equal("Test message", cheepDto.Text);
        Assert.Equal("2023-01-01 12:00:00", cheepDto.DatePosted);
        Assert.Equal("TestAuthor", cheepDto.AuthorName);
        Assert.Equal(1, cheepDto.ParentCheepID);
        Assert.Empty(cheepDto.Replies);
    }

    [Fact]
    public void CheepDTOEmptyStringsTest()
    {
        var cheepDto = new CheepDTO(
            Text: "",
            DatePosted: "",
            AuthorName: "",
            CheepId: 0,
            ParentCheepID: 1,
            Replies: new List<CheepDTO>(),
            Likes: "0"
        );

        Assert.Empty(cheepDto.Text);
        Assert.Empty(cheepDto.DatePosted);
        Assert.Empty(cheepDto.AuthorName);
    }

    [Fact]
    public void CheepDTOLongStringsTest()
    {
        var longText = new string('a', 500);
        var longDate = "2023-12-31 23:59:59.999";
        var longAuthor = new string('b', 100);

        var cheepDto = new CheepDTO(
            Text: longText,
            DatePosted: longDate,
            AuthorName: longAuthor,
            CheepId: 0,
            ParentCheepID: 1,
            Replies: new List<CheepDTO>(),
            Likes: "0"
        );

        Assert.Equal(500, cheepDto.Text.Length);
        Assert.Equal(longDate, cheepDto.DatePosted);
        Assert.Equal(100, cheepDto.AuthorName.Length);
    }

    [Fact]
    public void CheepDTOToStringTest()
    {
        var replyDTO = new CheepDTO(
            Text: "Hello World",
            DatePosted: "2023-01-01",
            AuthorName: "TestUser",
            CheepId: 0,
            ParentCheepID: 1,
            Replies: new List<CheepDTO>(),
            Likes: "0"
        );

        var cheepDto = new CheepDTO(
            Text: "Hello World",
            DatePosted: "2023-01-01",
            AuthorName: "TestUser",
            CheepId: 0,
            ParentCheepID: 1,
            Replies: new List<CheepDTO> { replyDTO },
            Likes: "0"
        );

        var result = cheepDto.ToString();

        Assert.Contains("Hello World", result);
        Assert.Contains("2023-01-01", result);
        Assert.Contains("TestUser", result);
        Assert.Contains("1", result);
        foreach (var reply in cheepDto.Replies)
        {
            Assert.Contains(replyDTO.ToString(), reply.ToString());
        }
    }
}
