using Chirp.Infrastructure.Services;

namespace Chirp.Web.Tests.DTO;

public class CheepDTOTests
{
    [Fact]
    public void CheepDTOTest()
    {
        var cheepDto = new CheepDTO("Test message", "2023-01-01 12:00:00", "TestAuthor", 0);

        Assert.Equal("Test message", cheepDto.Text);
        Assert.Equal("2023-01-01 12:00:00", cheepDto.DatePosted);
        Assert.Equal("TestAuthor", cheepDto.AuthorName);
    }

    [Fact]
    public void CheepDTOEmptyStringsTest()
    {
        var cheepDto = new CheepDTO("", "", "", 0);

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

        var cheepDto = new CheepDTO(longText, longDate, longAuthor, 0);

        Assert.Equal(500, cheepDto.Text.Length);
        Assert.Equal(longDate, cheepDto.DatePosted);
        Assert.Equal(100, cheepDto.AuthorName.Length);
    }

    [Fact]
    public void CheepDTOToStringTest()
    {
        var cheepDto = new CheepDTO("Hello World", "2023-01-01", "TestUser", 0);

        var result = cheepDto.ToString();

        Assert.Contains("Hello World", result);
        Assert.Contains("2023-01-01", result);
        Assert.Contains("TestUser", result);
    }
}
