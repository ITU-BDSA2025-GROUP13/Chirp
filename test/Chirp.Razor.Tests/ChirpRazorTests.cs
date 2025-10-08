using Chirp.Razor.Pages;
using Chirp.Razor;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
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
                DELETE FROM cheep;
                DELETE FROM author;

                INSERT INTO author (author_id, username, email)
                VALUES (1, 'Karl Fortnite', 'Karl.Fortnite@gmail.com');

                INSERT INTO cheep (message_id, author_id, text, pub_date)
                VALUES (1, 1, 'I love Fortnite', '2020-05-31 08:07:57');
            ";
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
                DELETE FROM cheep;
                DELETE FROM author;
            ";
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    // This tests the Razor service and its integration without DBFacade
    [Fact]
    public void PublicModel_OnGet_ShouldPopulateCheeps()
    {
        populateTable();

        var service = new CheepService();

        var pageModel = new PublicModel(service);

        pageModel.OnGet(0);

        Assert.NotNull(pageModel.Cheeps);
        Assert.Single(pageModel.Cheeps);
        Assert.Equal("I love Fortnite", pageModel.Cheeps[0].Text);

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
        Assert.Equal("Karl Fortnite", pageModel.Cheeps[0].AuthorName);
        Assert.Equal("I love Fortnite", pageModel.Cheeps[0].Text);

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

    /// This tests the Razor pages and what they display
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
