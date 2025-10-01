using Chirp.Models;
using Chirp.SQLite;
using Microsoft.Data.Sqlite;
using System.Linq;

public class ChirpSQLiteTests
{
    private DBFacade _db;
    private readonly string _dbPath = "/tmp/chirp/test.db";

    public ChirpSQLiteTests()
    {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", _dbPath);
        _db = new DBFacade();

        // Set up db for testing
        using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM message;
                DELETE FROM user;

                INSERT INTO user (user_id, username, email, pw_hash)
                VALUES (@1, 'Jacqualine Gilcoine', 'Jacqualine.Gilcoine@gmail.com', 'password');

                INSERT INTO user (user_id, username, email, pw_hash)
                VALUES (@2, 'Jhon Pork', 'Jhon.Pork@gmail.com', 'porkword');

                INSERT INTO user (user_id, username, email, pw_hash)
                VALUES (@3, 'Karl Fortnight', 'Karl.Fortnight@gmail.com', 'word');

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (1, @1, 'hello world', 1790895677);

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (2, @2, 'hello pork world', 1690895677);

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (3, @3, 'I love Fortnight', 1590895677);
            ";

            command.Parameters.AddWithValue("@1", "Jacqualine Gilcoine".GetHashCode());
            command.Parameters.AddWithValue("@2", "Jhon Pork".GetHashCode());
            command.Parameters.AddWithValue("@3", "Karl Fortnight".GetHashCode());

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    [Fact]
    public void ReadAllMessageTest()
    {
        List<CheepViewModel> cheeps = _db.Read().ToList();

        Assert.Equal("hello world", cheeps[0].Message);
        Assert.Equal("Jacqualine Gilcoine", cheeps[0].Author);

        Assert.Equal("hello pork world", cheeps[1].Message);
        Assert.Equal("Jhon Pork", cheeps[1].Author);

        Assert.Equal("I love Fortnight", cheeps[2].Message);
        Assert.Equal("Karl Fortnight", cheeps[2].Author);
    }

    [Fact]
    public void ReadSomeMessageTest()
    {
        List<CheepViewModel> cheeps = _db.Read(null, 1).ToList();

        Assert.Equal("hello world", cheeps[0].Message);
        Assert.Equal("Jacqualine Gilcoine", cheeps[0].Author);

        Assert.Single(cheeps);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            CheepViewModel cheep = cheeps[2];
        });
    }

    [Fact]
    public void ReadUserMessagesTest()
    {
        List<CheepViewModel> cheeps = _db.Read("Karl Fortnight", 0).ToList();

        Assert.Equal("I love Fortnight", cheeps[0].Message);
        Assert.Equal("Karl Fortnight", cheeps[0].Author);

        Assert.Single(cheeps);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            CheepViewModel cheep = cheeps[2];
        });
    }

    [Fact]
    public void CreateMessage()
    {
        var newCheep = new CheepViewModel("Karl Fortnight", "Mannnnnnn I Love Fortnight", "1490895677");
        _db.Create(newCheep);

        List<CheepViewModel> cheeps = _db.Read("Karl Fortnight", 0).ToList();

        Assert.False(1 == cheeps.Count);

        Assert.Equal("I love Fortnight", cheeps[0].Message);
        Assert.Equal("Karl Fortnight", cheeps[0].Author);

        Assert.Equal("Mannnnnnn I Love Fortnight", cheeps[1].Message);
        Assert.Equal("Karl Fortnight", cheeps[1].Author);
    }
}
