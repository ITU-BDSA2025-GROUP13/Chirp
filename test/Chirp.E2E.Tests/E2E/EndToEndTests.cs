using Chirp.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright.Helpers;
using Microsoft.Playwright.Xunit;
using System.Text.RegularExpressions;

namespace Chirp.Integration.Tests.E2E;

[Collection("Playwright collection")]
public class EndToEndTests : PageTest, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ChirpEndToEndPlaywrightFixture _fixture;
    private readonly string _baseUrl;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public EndToEndTests(WebApplicationFactory<Program> factory, ChirpEndToEndPlaywrightFixture fixture)
    {
        _fixture = fixture;
        _baseUrl = fixture._baseUrl;
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
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

    [Fact]
    public async Task User_Can_Post_And_Edit_Cheep()
    {
        string testEmail = $"testuser_{Guid.NewGuid()}@example.com";
        string testPassword = "TestPassword123!";
        string testUser = $"testuser_{Guid.NewGuid()}";
        string message = $"This is a message {Guid.NewGuid()}";
        string editedMessage = $"This is an edited message {Guid.NewGuid()}";

        // Register
        await Page.GotoAsync($"{_baseUrl}/Identity/Account/Register?ReturnUrl=%2FIdentity%2FAccount%2FLogout");
        await Page.FillAsync("#Input_Email", testEmail);
        await Page.FillAsync("#Input_UserName", testUser);
        await Page.FillAsync("#Input_Password", testPassword);
        await Page.FillAsync("#Input_ConfirmPassword", testPassword);
        await Page.ClickAsync("button[type='submit']");

        // Post cheep
        await Page.GotoAsync($"{_baseUrl}/");
        await Page.FillAsync(".post-textarea", message);
        await Page.ClickAsync("input[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        // Assert message has been posted
        await Expect(Page.Locator(".cheep-text", new()
        {
            HasTextString = message
        })).ToBeVisibleAsync();

        // Edit cheep
        await Page.ClickAsync("button:text('Edit')");
        await Page.FillAsync(".cheep-edit-input", editedMessage);
        await Page.ClickAsync("input:text('Finish')");

        // Assert message has been edited
        await Expect(Page.Locator(".cheep-text", new()
        {
            HasTextString = editedMessage
        })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task User_Cannot_Type_More_Than_160_Characters_UI()
    {
        string testUser = $"{Guid.NewGuid()}";
        string testEmail = $"{testUser}@example.com";
        string testPassword = "TestPassword123!";
        int maxCharcount = Chirp.Web.Constants.Constants.MaxCheepLength;
        string message = new string('*', maxCharcount + 1);
        string expectedMessage = message.Remove(message.Length - 1);

        // Register
        await Page.GotoAsync($"{_baseUrl}/Identity/Account/Register?ReturnUrl=%2FIdentity%2FAccount%2FLogout");
        await Page.FillAsync("#Input_Email", testEmail);
        await Page.FillAsync("#Input_UserName", testUser);
        await Page.FillAsync("#Input_Password", testPassword);
        await Page.FillAsync("#Input_ConfirmPassword", testPassword);
        await Page.ClickAsync("button[type='submit']");
        await Page.GotoAsync($"{_baseUrl}/");

        // Assert full charcount
        await Expect(Page.Locator(".post-form-charcount")).ToHaveTextAsync($"{maxCharcount} characters left");

        // Type message that is too long
        await Page.FillAsync(".post-textarea", message);
        await Expect(Page.Locator(".post-textarea")).ToHaveValueAsync(expectedMessage);
        await Expect(Page.Locator(".post-form-charcount")).ToHaveTextAsync("0 characters left");
    }
}
