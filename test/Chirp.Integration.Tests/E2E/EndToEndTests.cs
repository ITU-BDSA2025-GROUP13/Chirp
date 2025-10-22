using Chirp.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Chirp.Integration.Tests.E2E;

public class EndToEndTests
{
    private WebApplicationFactory<Program>? _factory;

    private void populateTable()
    {
        // this runs the program as is, it doesnt use an in memory database as we need program options to make it so
        Environment.SetEnvironmentVariable("DB_PATH", $"{Path.GetTempPath()}/chirp/e2eTest.db");
        _factory = new WebApplicationFactory<Program>();
    }

    [Fact]
    public async Task Get_PublicTimeline_ReturnsSuccessAndContainsExpectedContent()
    {
        populateTable();

        if (_factory is null)
        {
            throw new InvalidOperationException("HttpClientFactory is not available.");
        }

        HttpClient client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Public Timeline");
        html.Should().Contain("Starbuck now is what we hear the worst."); // this is in the DBinit
        html.Should().Contain("<a href=\"/Jacqualine Gilcoine\">");
    }

    [Fact]
    public async Task Get_UserTimeline_ReturnsSuccessAndContainsExpectedContent()
    {
        populateTable();

        if (_factory is null)
        {
            throw new InvalidOperationException("HttpClientFactory is not available.");
        }

        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Jacqualine Gilcoine");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Jacqualine Gilcoine's Timeline");
        html.Should().Contain("Starbuck now is what we hear the worst."); // this is in the DBinit
        html.Should().Contain("<a href=\"/Jacqualine Gilcoine\">");
    }
}
