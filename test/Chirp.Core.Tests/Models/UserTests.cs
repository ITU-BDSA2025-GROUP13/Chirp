using Chirp.Core.Models;

namespace Chirp.Core.Tests.Models;

public class UserTests
{
    [Fact]
    public void Properties_WhenSet_HoldAssignedValues()
    {
        string name = "TestUser";
        string email = "test@example.com";

        var author = new ChirpUser();

        author.UserName = name;
        author.Email = email;

        Assert.Equal(name, author.UserName);
        Assert.Equal(email, author.Email);
    }

    [Fact]
    public void Cheeps_WhenItemIsAdded_ContainsTheItem()
    {
        var author = new ChirpUser();
        var cheep = new Cheep { TimeStamp = DateTime.Now, AuthorId = "0", Author = author, Text = "hello" };

        author.Cheeps.Add(cheep);

        Assert.Single(author.Cheeps);
        Assert.Contains(cheep, author.Cheeps);
    }
}
