using Chirp.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright.Xunit;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace Chirp.Integration.Tests.E2E;

public class EndToEndTests : PageTest, IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private static Process? _serverProcess;
    private static bool _serverStarted = false;
    private static readonly string dbPath = $"{Path.GetTempPath()}/chirp/e2eTest.db";
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

        startInfo.Environment["CHIRPDBPATH"] = dbPath;
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

    [Fact]
    public async Task User_CanRegister_Login_And_Logout()
    {
        string testEmail = $"testuser@example.com";
        string testPassword = "TestPassword123!";
        string testUser = $"testuser";

        // Register
        await Page.GotoAsync($"{_baseUrl}/Identity/Account/Register?ReturnUrl=%2FIdentity%2FAccount%2FLogout");
        await Page.FillAsync("#Input_Email", testEmail);
        await Page.FillAsync("#Input_UserName", testUser);
        await Page.FillAsync("#Input_Password", testPassword);
        await Page.FillAsync("#Input_ConfirmPassword", testPassword);
        await Page.ClickAsync("button[type='submit']");
        // Verify registration success 
        await Page.WaitForURLAsync(new Regex(".*Identity/Account/Logout.*"));
        Assert.Contains("/Identity/Account/Logout", Page.Url);
        var PostRegistrationContent = await Page.ContentAsync();
        Assert.True(PostRegistrationContent.Contains("Logout") || PostRegistrationContent.Contains("logout") || PostRegistrationContent.Contains("Click here to Logout"));
        Console.WriteLine("✓ Registration successful");

        // Logout
        await Page.ClickAsync("button[type='submit']");
        Assert.DoesNotContain("/Identity/Account/Logout", Page.Url);
        // Verify logged out success
        await Page.WaitForURLAsync(new Regex(".*Identity/Account/Login.*"));
        Assert.Contains("/Identity/Account/Login", Page.Url);
        Console.WriteLine("✓ Logout successful");

        // Login
        await Page.GotoAsync($"{_baseUrl}/Identity/Account/Login?ReturnUrl=%2FIdentity%2FAccount%2FLogout");
        await Page.FillAsync("#Input_UserName", testUser);
        await Page.FillAsync("#Input_Password", testPassword);
        await Page.ClickAsync("button[type='submit']");
        Assert.DoesNotContain("/Identity/Account/Login", Page.Url);
        // Verify logged in success
        await Page.WaitForURLAsync(new Regex(".*Identity/Account/Logout.*"));
        Assert.Contains("/Identity/Account/Logout", Page.Url);
        var PostLoginContent = await Page.ContentAsync();
        Assert.True(PostLoginContent.Contains("Logout") || PostLoginContent.Contains("logout") || PostLoginContent.Contains("Click here to Logout"));
        Console.WriteLine("✓ Login successful");
    }

    /// <summary>
    /// Clean up database after tests have run
    /// </summary>
    public void Dispose()
    {
        File.Delete(dbPath);
    }
}
