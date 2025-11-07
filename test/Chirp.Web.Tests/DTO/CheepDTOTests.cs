using Chirp.Infrastructure.Services;

namespace Chirp.Web.Tests.DTO;

public class CheepDTOTests
{
    [Fact]
    public void CheepDTOTest()
    {
        var cheepDto = new CheepDTO("Test message", "2023-01-01 12:00:00", "TestAuthor");

        Assert.Equal("Test message", cheepDto.Text);
        Assert.Equal("2023-01-01 12:00:00", cheepDto.DatePosted);
        Assert.Equal("TestAuthor", cheepDto.AuthorName);
    }

    [Fact]
    public void CheepDTOEqualityComparisonTest()
    {
        var cheep1 = new CheepDTO("Same message", "2023-01-01", "SameAuthor");
        var cheep2 = new CheepDTO("Same message", "2023-01-01", "SameAuthor");
        var cheep3 = new CheepDTO("Different message", "2023-01-01", "SameAuthor");

        Assert.Equal(cheep2, cheep1);
        Assert.NotEqual(cheep3, cheep1);
        Assert.True(cheep1.Equals(cheep2));
        Assert.False(cheep1.Equals(cheep3));
    }

    [Fact]
    public void CheepDTOEmptyStringsTest()
    {
        var cheepDto = new CheepDTO("", "", "");

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

        var cheepDto = new CheepDTO(longText, longDate, longAuthor);

        Assert.Equal(500, cheepDto.Text.Length);
        Assert.Equal(longDate, cheepDto.DatePosted);
        Assert.Equal(100, cheepDto.AuthorName.Length);
    }

    [Fact]
    public void CheepDTOGetHashCodeTest()
    {
        var cheep1 = new CheepDTO("Test", "2023-01-01", "Author");
        var cheep2 = new CheepDTO("Test", "2023-01-01", "Author");

        Assert.Equal(cheep2.GetHashCode(), cheep1.GetHashCode());
    }

    [Fact]
    public void CheepDTOToStringTest()
    {
        var cheepDto = new CheepDTO("Hello World", "2023-01-01", "TestUser");

        var result = cheepDto.ToString();

        Assert.Contains("Hello World", result);
        Assert.Contains("2023-01-01", result);
        Assert.Contains("TestUser", result);
    }
}
