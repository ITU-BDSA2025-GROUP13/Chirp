using Xunit;
using Chirp.Razor.Pages;
using Chirp.Models;
using Chirp.SQLite;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class ChirpRazorTests
{
    private DBFacade _db;
    private readonly string _dbPath = "/tmp/chirp/test.db";

    public ChirpRazorTests()
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
                VALUES (@1, 'Karl Fortnite', 'Karl.Fortnite@gmail.com', 'word');

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (1, @1, 'I love Fortnite', 1590895677);
            ";

            command.Parameters.AddWithValue("@1", "Karl Fortnite".GetHashCode());

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    private void clearTables()
    {
        using (var connection = new SqliteConnection($"Data Source={_dbPath}"))
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM message;
                DELETE FROM user;
            ";

            connection.Open();
            command.ExecuteNonQuery();
        }
    }



    [Fact]
    public void PublicModel_OnGet_ShouldPopulateCheeps()
    {

        var service = new CheepService();

        var pageModel = new PublicModel(service);

        var result = pageModel.OnGet(0);

        Assert.NotNull(pageModel.Cheeps);
        Assert.Single(pageModel.Cheeps);
        Assert.Equal("I love Fortnite", pageModel.Cheeps[0].Message);
    }

    [Fact]
    public void UserTimelineModel_OnGet_ShouldPopulateCheepsFromAuthor()
    {
        var service = new CheepService();

        var pageModel = new UserTimelineModel(service);

        var result = pageModel.OnGet("Karl Fortnite", 0);

        Assert.NotNull(pageModel.Cheeps);
        Assert.Single(pageModel.Cheeps);
        Assert.Equal("Karl Fortnite", pageModel.Cheeps[0].Author);
        Assert.Equal("I love Fortnite", pageModel.Cheeps[0].Message);
    }

    [Fact]
    public void PublicModel_OnGet_ShouldBeEmptyWithNoCheeps()
    {

        clearTables();

        var service = new CheepService();

        var pageModel = new PublicModel(service);

        var result = pageModel.OnGet(0);

        Assert.Equal(new List<CheepViewModel>(), pageModel.Cheeps);
        Assert.Empty(pageModel.Cheeps);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            string cheepMessage = pageModel.Cheeps[0].Message;
        });
    }

    [Fact]
    public void UserTimelineModel_OnGet_ShouldBeNullIfNoAuthor()
    {
        clearTables();

        var service = new CheepService();

        var pageModel = new UserTimelineModel(service);

        var result = pageModel.OnGet("Karl Fortnite", 0);

        Assert.Equal(new List<CheepViewModel>(), pageModel.Cheeps);

        Assert.Empty(pageModel.Cheeps);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            string Author = pageModel.Cheeps[0].Author;
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            string cheepMessage = pageModel.Cheeps[0].Message;
        });
    }
}
