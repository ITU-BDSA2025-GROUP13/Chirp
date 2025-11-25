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
        await Page.FillAsync("#post-text-field", message);
        await Page.ClickAsync("input[type='submit']");
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        // Assert message has been posted
        var cheep = Page.Locator("li.cheep", new() { HasTextString = message });
        await Expect(cheep).ToBeVisibleAsync(new());

        var editButton = cheep.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Edit" });
        await Expect(editButton).ToBeVisibleAsync(new());
        await editButton.ClickAsync();

        var editInput = cheep.Locator("input[name='EditedCheepText']");
        await Expect(editInput).ToBeVisibleAsync(new());
        await editInput.FillAsync(editedMessage);

        var finishButton = cheep.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Finish" });
        await Expect(finishButton).ToBeVisibleAsync(new());
        await finishButton.ClickAsync();
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

        // Assert message has been edited
        await Expect(Page.GetByText(editedMessage)).ToBeVisibleAsync(new());
    }
}
