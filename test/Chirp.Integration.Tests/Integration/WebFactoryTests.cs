using Chirp.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

public class WebFactoryTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public WebFactoryTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_PublicTimeline_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Public Timeline", html);
        Assert.Contains("Starbuck now is what we hear the worst.", html); // this is in the DBinit
        Assert.Contains("<a href=\"/user/Jacqualine Gilcoine\">", html);
    }

    [Fact]
    public async Task Get_UserTimeline_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/user/Jacqualine Gilcoine");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Jacqualine Gilcoine's Timeline", html);
        Assert.Contains("Starbuck now is what we hear the worst.", html); // this is in the DBinit
        Assert.Contains("<a href=\"/user/Jacqualine Gilcoine\">", html);
    }

    [Fact]
    public async Task Get_Login_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/Identity/Account/Login");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Login", html);
        Assert.Contains("login-submit", html);
    }

    [Fact]
    public async Task Get_Register_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/Identity/Account/Register");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Register", html);
        Assert.Contains("Create a new account", html);
    }

    [Fact]
    public async Task Get_Logout_ReturnsSuccessOrRedirect()
    {
        var response = await _client.GetAsync("/Identity/Account/Logout");

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Redirect);
    }
}
