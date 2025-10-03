using Xunit;
using Chirp.Razor.Pages;
using Chirp.Razor;
using Chirp.Domain;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;

public class ChirpRazorTests
{
    private CheepService _cheepService;
    private readonly string _databasePath = "/tmp/chirp/razorTest.db";
    private readonly WebApplicationFactory<Program> _factory;

    public ChirpRazorTests()
    {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", _databasePath);

        _cheepService = new CheepService();

        _factory = new WebApplicationFactory<Program>();
    }

    private void populateTable()
    {
        using (var connection = new SqliteConnection($"Data Source={_databasePath}"))
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
        using (var connection = new SqliteConnection($"Data Source={_databasePath}"))
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

    // This tests the Razor service and its intgration with out DBFacade

    [Fact]
    public void PublicModel_OnGet_ShouldPopulateCheeps()
    {
        populateTable();

        var service = new CheepService();

        var pageModel = new PublicModel(service);

        pageModel.OnGet(0);

        Assert.NotNull(pageModel.Cheeps);
        Assert.Single(pageModel.Cheeps);
        Assert.Equal("I love Fortnite", pageModel.Cheeps[0].Message);

        clearTables();
    }

    [Fact]
    public void UserTimelineModel_OnGet_ShouldPopulateCheepsFromAuthor()
    {
        populateTable();

        var service = new CheepService();

        var pageModel = new UserTimelineModel(service);

        pageModel.OnGet("Karl Fortnite", 0);

        Assert.NotNull(pageModel.Cheeps);
        Assert.Single(pageModel.Cheeps);
        Assert.Equal("Karl Fortnite", pageModel.Cheeps[0].Author);
        Assert.Equal("I love Fortnite", pageModel.Cheeps[0].Message);

        clearTables();
    }

    [Fact]
    public void PublicModel_OnGet_ShouldBeEmptyWithNoCheeps()
    {
        populateTable();

        clearTables();

        var service = new CheepService();

        var pageModel = new PublicModel(service);

        pageModel.OnGet(0);

        Assert.Empty(pageModel.Cheeps);
    }

    [Fact]
    public void UserTimelineModel_OnGet_ShouldBeNullIfNoAuthor()
    {
        populateTable();

        clearTables();

        var service = new CheepService();

        var pageModel = new UserTimelineModel(service);

        pageModel.OnGet("Karl Fortnite", 0);

        Assert.Empty(pageModel.Cheeps);
    }

    // This tests the Razor pages and what they display

    [Fact]
    public async Task PublicPage_ShouldRenderCheeps()
    {
        populateTable();

        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Public Timeline", html);
        Assert.Contains("I love Fortnite", html);
        Assert.Contains("<a href=\"/Karl Fortnite\">", html);

        clearTables();
    }

    [Fact]
    public async Task UserTimelinePage_ShouldRenderCheeps()
    {
        populateTable();

        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Karl Fortnite");
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Karl Fortnite's Timeline", html);
        Assert.Contains("I love Fortnite", html);
        Assert.Contains("<a href=\"/Karl Fortnite\">", html);

        clearTables();
    }


    [Fact]
    public async Task PublicPage_ShouldNotRenderCheepsIfEmpty()
    {
        populateTable();

        clearTables();

        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Public Timeline", html);
        Assert.DoesNotContain("I love Fortnite", html);
        Assert.DoesNotContain("<a href=\"/Karl Fortnite\">", html);

    }

    [Fact]
    public async Task UserTimelinePage_ShouldNotRenderCheepsIfEmpty()
    {
        populateTable();

        clearTables();

        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Karl Fortnite");

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("Karl Fortnite's Timeline", html);
        Assert.DoesNotContain("I love Fortnite", html);
        Assert.DoesNotContain("<a href=\"/Karl Fortnite\">", html);

    }

}
