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
                DELETE FROM cheep;
                DELETE FROM author;

                INSERT INTO author (author_id, username, email)
                VALUES (1, 'Jacqualine Gilcoine', 'Jacqualine.Gilcoine@gmail.com');

                INSERT INTO author (author_id, username, email)
                VALUES (2, 'John Pork', 'John.Pork@gmail.com');

                INSERT INTO author (author_id, username, email)
                VALUES (3, 'Karl Fortnite', 'Karl.Fortnite@gmail.com');

                INSERT INTO cheep (message_id, author_id, text, pub_date)
                VALUES (1, 1, 'hello world', '2026-09-30 12:34:37');

                INSERT INTO cheep (message_id, author_id, text, pub_date)
                VALUES (2, 2, 'hello pork world', '2023-08-01 10:21:17');

                INSERT INTO cheep (message_id, author_id, text, pub_date)
                VALUES (3, 3, 'I love Fortnite', '2020-05-31 08:07:57');
            ";
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    [Fact]
    public void ReadAllMessageTest()
    {
        List<Cheep> cheeps = _db.ReadPage().ToList();

        Assert.Equal("hello world", cheeps[0].Text);
        Assert.Equal("Jacqualine Gilcoine", cheeps[0].Author.Name);

        Assert.Equal("hello pork world", cheeps[1].Text);
        Assert.Equal("John Pork", cheeps[1].Author.Name);

        Assert.Equal("I love Fortnite", cheeps[2].Text);
        Assert.Equal("Karl Fortnite", cheeps[2].Author.Name);
    }

    [Fact]
    public void ReadUserMessagesTest()
    {
        Author author = _db.ReadPageFromAuthor("Karl Fortnite");
        List<Cheep> cheeps = author.Cheeps.ToList();

        Assert.Equal("I love Fortnite", cheeps[0].Text);
        Assert.Equal("Karl Fortnite", cheeps[0].Author.Name);

        Assert.Single(cheeps); 

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Cheep cheep = cheeps[2];
        });
    }

    [Fact]
    public void CreateMessage()
    {
        var author = _db.GetAuthorFromUsername("Karl Fortnite");
        var newCheep = new Cheep("Mannnnnnn I Love Fortnite", DateTime.UtcNow, author);
        _db.Create(newCheep);
        
        List<Cheep> cheeps = _db.ReadPageFromAuthor("Karl Fortnite").Cheeps.ToList();

        Assert.Equal(2, cheeps.Count);

        Assert.Equal("Mannnnnnn I Love Fortnite", cheeps[0].Text);
        Assert.Equal(author.Name, cheeps[0].Author.Name);

        Assert.Equal("I love Fortnite", cheeps[1].Text);
        Assert.Equal(author.Name, cheeps[1].Author.Name);
    }
}
