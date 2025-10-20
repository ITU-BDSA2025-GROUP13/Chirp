using FluentAssertions;

namespace Chirp.Razor.Tests;

public class CheepDTOTests
{
    [Fact]
    public void CheepDTOTest()
    {
        var cheepDto = new CheepDTO("Test message", "2023-01-01 12:00:00", "TestAuthor");

        cheepDto.Text.Should().Be("Test message");
        cheepDto.DatePosted.Should().Be("2023-01-01 12:00:00");
        cheepDto.AuthorName.Should().Be("TestAuthor");
    }

    [Fact]
    public void CheepDTOEqualityComparisonTest()
    {
        var cheep1 = new CheepDTO("Same message", "2023-01-01", "SameAuthor");
        var cheep2 = new CheepDTO("Same message", "2023-01-01", "SameAuthor");
        var cheep3 = new CheepDTO("Different message", "2023-01-01", "SameAuthor");

        cheep1.Should().Be(cheep2);
        cheep1.Should().NotBe(cheep3);
        cheep1.Equals(cheep2).Should().BeTrue();
        cheep1.Equals(cheep3).Should().BeFalse();
    }

    [Fact]
    public void CheepDTOEmptyStringsTest()
    {
        var cheepDto = new CheepDTO("", "", "");

        cheepDto.Text.Should().BeEmpty();
        cheepDto.DatePosted.Should().BeEmpty();
        cheepDto.AuthorName.Should().BeEmpty();
    }

    [Fact]
    public void CheepDTOLongStringsTest()
    {
        var longText = new string('a', 500);
        var longDate = "2023-12-31 23:59:59.999";
        var longAuthor = new string('b', 100);

        var cheepDto = new CheepDTO(longText, longDate, longAuthor);

        cheepDto.Text.Should().HaveLength(500);
        cheepDto.DatePosted.Should().Be(longDate);
        cheepDto.AuthorName.Should().HaveLength(100);
    }

    [Fact]
    public void CheepDTOGetHashCodeTest()
    {
        var cheep1 = new CheepDTO("Test", "2023-01-01", "Author");
        var cheep2 = new CheepDTO("Test", "2023-01-01", "Author");

        cheep1.GetHashCode().Should().Be(cheep2.GetHashCode());
    }

    [Fact]
    public void CheepDTOToStringTest()
    {
        var cheepDto = new CheepDTO("Hello World", "2023-01-01", "TestUser");

        var result = cheepDto.ToString();

        result.Should().Contain("Hello World");
        result.Should().Contain("2023-01-01");
        result.Should().Contain("TestUser");
    }
}
