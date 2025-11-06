using Chirp.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright.Xunit;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Chirp.Integration.Tests.E2E;

public class EndToEndTests : PageTest, IClassFixture<WebApplicationFactory<Program>>
{
    private static Process? _serverProcess;
    private static bool _serverStarted = false;
    private readonly string _baseUrl = "http://localhost:5273";
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public EndToEndTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        if (!_serverStarted)
        {
            StartServer();
            _serverStarted = true;
        }
    }

    private static void StartServer()
    {
        var projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "src", "Chirp.Web"));

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "run --no-build --urls http://localhost:5273",
            WorkingDirectory = projectDir,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.Environment["CHIRPDBPATH"] = $"{Path.GetTempPath()}/chirp/e2eTest.db";
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Testing";

        _serverProcess = Process.Start(startInfo);

        Thread.Sleep(5000);
    }

    [Fact]
    public async Task Get_PublicTimeline_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Public Timeline");
        html.Should().Contain("Starbuck now is what we hear the worst."); // this is in the DBinit
        html.Should().Contain("<a href=\"/user/Jacqualine Gilcoine\">");
    }

    [Fact]
    public async Task Get_UserTimeline_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/user/Jacqualine Gilcoine");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Jacqualine Gilcoine's Timeline");
        html.Should().Contain("Starbuck now is what we hear the worst."); // this is in the DBinit
        html.Should().Contain("<a href=\"/user/Jacqualine Gilcoine\">");
    }

    [Fact]
    public async Task Get_Login_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/Identity/Account/Login");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Login");
        html.Should().Contain("login-submit");
    }

    [Fact]
    public async Task Get_Register_ReturnsSuccessAndContainsExpectedContent()
    {
        var response = await _client.GetAsync("/Identity/Account/Register");

        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Register");
        html.Should().Contain("Create a new account");
    }

    [Fact]
    public async Task Get_Logout_ReturnsSuccessOrRedirect()
    {
        var response = await _client.GetAsync("/Identity/Account/Logout");

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task User_CanRegister_Login_And_Logout()
    {
        string testEmail = $"testuser_{Guid.NewGuid()}@example.com";
        string testPassword = "TestPassword123!";
        string testUser = $"testuser_{Guid.NewGuid()}";

        // Register
        await Page.GotoAsync($"{_baseUrl}/Identity/Account/Register?ReturnUrl=%2FIdentity%2FAccount%2FLogout");
        await Page.FillAsync("#Input_Email", testEmail);
        await Page.FillAsync("#Input_UserName", testUser);
        await Page.FillAsync("#Input_Password", testPassword);
        await Page.FillAsync("#Input_ConfirmPassword", testPassword);
        await Page.ClickAsync("button[type='submit']");
        // Verify registration success 
        await Page.WaitForURLAsync(new Regex(".*Identity/Account/Logout.*"));
        Page.Url.Should().Contain("/Identity/Account/Logout");
        var PostRegistrationContent = await Page.ContentAsync();
        PostRegistrationContent.Should().ContainAny("Logout", "logout", "Click here to Logout");
        Console.WriteLine("✓ Registration successful");

        // Logout
        await Page.ClickAsync("button[type='submit']");
        Page.Url.Should().NotContain("/Identity/Account/Logout");
        // Verify logged out success
        await Page.WaitForURLAsync(new Regex(".*Identity/Account/Login.*"));
        Page.Url.Should().Contain("/Identity/Account/Login");
        Console.WriteLine("✓ Logout successful");

        // Login
        await Page.GotoAsync($"{_baseUrl}/Identity/Account/Login?ReturnUrl=%2FIdentity%2FAccount%2FLogout");
        await Page.FillAsync("#Input_UserName", testUser);
        await Page.FillAsync("#Input_Password", testPassword);
        await Page.ClickAsync("button[type='submit']");
        Page.Url.Should().NotContain("/Identity/Account/Login");
        // Verify logged in success
        await Page.WaitForURLAsync(new Regex(".*Identity/Account/Logout.*"));
        Page.Url.Should().Contain("/Identity/Account/Logout");
        var PostLoginContent = await Page.ContentAsync();
        PostLoginContent.Should().ContainAny("Logout", "logout", "Click here to Logout");
        Console.WriteLine("✓ Login successful");
    }
}
