using Chirp.Models;
using Chirp.SQLite;
using Microsoft.Data.Sqlite;
using System.Linq;

public class ChirpSQLiteTests
{
    [Fact]
    public void ReadAllMessageTest()
    {
        var db = CreatesDatabase();
        List<CheepViewModel> cheeps = db.Read().ToList();

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
        var db = CreatesDatabase();
        List<CheepViewModel> cheeps = db.Read(null, 1).ToList();

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
        var db = CreatesDatabase();
        List<CheepViewModel> cheeps = db.Read("Karl Fortnight", 0).ToList();

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
        var db = CreatesDatabase();
        var newCheep = new CheepViewModel("Karl Fortnight", "Mannnnnnn I Love Fortnight", "1490895677");
        db.Create(newCheep);

        List<CheepViewModel> cheeps = db.Read("Karl Fortnight", 0).ToList();

        Assert.False(1 == cheeps.Count);

        Assert.Equal("I love Fortnight", cheeps[0].Message);
        Assert.Equal("Karl Fortnight", cheeps[0].Author);

        Assert.Equal("Mannnnnnn I Love Fortnight", cheeps[1].Message);
        Assert.Equal("Karl Fortnight", cheeps[1].Author);

    }

    private IChirpFacade CreatesDatabase()
    {
        var connectionString = "Data Source=:memory:;Cache=Shared";
        var db = new DBFacade(connectionString);

        using var connection = new SqliteConnection($"Data Source={connectionString}");
        using var command = connection.CreateCommand();
        command.CommandText = @"

                drop table if exists user;
                create table user (
                  user_id integer primary key autoincrement,
                  username string not null,
                  email string not null,
                  pw_hash string not null
                );

                drop table if exists message;
                create table message (
                  message_id integer primary key autoincrement,
                  author_id integer not null,
                  text string not null,
                  pub_date integer
                );

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
        int rowsAffected = command.ExecuteNonQuery();

        return db;
    }
}
