using Chirp.Domain;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Linq;

public class ChirpSQLiteTests
{
    private readonly string _databasePath = "/tmp/chirp/test.db";

    private CheepService _cheepService;

    public ChirpSQLiteTests()
    {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", _databasePath);

        _cheepService = new CheepService();

        // Set up db for testing
        using (var connection = new SqliteConnection($"Data Source={_databasePath}"))
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM message;
                DELETE FROM user;

                INSERT INTO user (user_id, username, email, pw_hash)
                VALUES (@1, 'Jacqualine Gilcoine', 'Jacqualine.Gilcoine@gmail.com', 'password');

                INSERT INTO user (user_id, username, email, pw_hash)
                VALUES (@2, 'John Pork', 'John.Pork@gmail.com', 'porkword');

                INSERT INTO user (user_id, username, email, pw_hash)
                VALUES (@3, 'Karl Fortnite', 'Karl.Fortnite@gmail.com', 'word');

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (1, @1, 'hello world', 1790895677);

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (2, @2, 'hello pork world', 1690895677);

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (3, @3, 'I love Fortnite', 1590895677);
            ";

            command.Parameters.AddWithValue("@1", "Jacqualine Gilcoine".GetHashCode());
            command.Parameters.AddWithValue("@2", "John Pork".GetHashCode());
            command.Parameters.AddWithValue("@3", "Karl Fortnite".GetHashCode());

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    [Fact]
    public void ReadAllMessageTest()
    {
        List<Cheep> cheeps = _cheepService.GetCheeps();

        Assert.Equal("hello world", cheeps[0].Message);
        Assert.Equal("Jacqualine Gilcoine", cheeps[0].Author);

        Assert.Equal("hello pork world", cheeps[1].Message);
        Assert.Equal("John Pork", cheeps[1].Author);

        Assert.Equal("I love Fortnite", cheeps[2].Message);
        Assert.Equal("Karl Fortnite", cheeps[2].Author);
    }

    [Fact]
    public void ReadUserMessagesTest()
    {
        List<Cheep> cheeps = _cheepService.GetCheepsFromAuthor("Karl Fortnite");

        Assert.Equal("I love Fortnite", cheeps[0].Message);
        Assert.Equal("Karl Fortnite", cheeps[0].Author);

        Assert.Single(cheeps);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Cheep cheep = cheeps[2];
        });
    }

    [Fact]
    public void CreateMessage()
    {
        var newCheep = new Cheep("Karl Fortnite", "Mannnnnnn I Love Fortnite", "1490895677");
        _cheepService.PostCheep(newCheep);

        List<Cheep> cheeps = _cheepService.GetCheepsFromAuthor("Karl Fortnite");

        Assert.False(1 == cheeps.Count);

        Assert.Equal("I love Fortnite", cheeps[0].Message);
        Assert.Equal("Karl Fortnite", cheeps[0].Author);

        Assert.Equal("Mannnnnnn I Love Fortnite", cheeps[1].Message);
        Assert.Equal("Karl Fortnite", cheeps[1].Author);
    }
}
