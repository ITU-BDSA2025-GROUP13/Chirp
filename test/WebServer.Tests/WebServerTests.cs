using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit.Abstractions;
using Xunit.Sdk;


public class WebServerTests
{
    private readonly HttpClient client;

    public WebServerTests()
    {
        client = new WebApplicationFactory<Program>().CreateClient();
    }


    [Fact]
    public async void readFromServer_ResultOK()
    {
        var task = client.GetAsync("/cheeps");
        var response = await task;
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async void writeToServer_ResultOK()
    {
        var task = client.GetAsync("/cheep/writeTest");
        var response = await task;
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async void writeAndRead_ResultOK()
    {
        var response = await client.GetAsync("/cheep/writeAndReadTest");
        Assert.True(response.IsSuccessStatusCode);
        response = await client.GetAsync("/cheeps");
        Assert.True(response.IsSuccessStatusCode);
    }
}