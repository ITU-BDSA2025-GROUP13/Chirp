using Chirp.CLI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleDB;


public class WebServerTests
{
    private readonly HttpClient client;
    private readonly CSVDatabase<Cheep> database;
    private readonly string sampleDB = "../../../assets/testSample.csv";
    
    public WebServerTests()
    {
        client = new WebApplicationFactory<WebServer.Program>().CreateClient();
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
        string writeTestPath = "../../../assets/writeTest.csv";
        File.Copy(sampleDB, writeTestPath);
        CSVDatabase<Cheep>.SetPathForTest(writeTestPath);
        
        var task = client.PostAsync("/cheep/tester/writeTest", null);
        var response = await task;
        Assert.True(response.IsSuccessStatusCode);
        
        File.Delete(writeTestPath);
        CSVDatabase<Cheep>.SetPathForTest(sampleDB);
    }

    [Fact]
    public async void writeAndRead_ResultOK()
    {
        string writeTestPath = "../../../assets/writeTest.csv";
        File.Copy(sampleDB, writeTestPath);
        CSVDatabase<Cheep>.SetPathForTest(writeTestPath);
        
        var response = await client.PostAsync("/cheep/tester/writeAndReadTest", null);
        Assert.True(response.IsSuccessStatusCode);
        response = await client.GetAsync("/cheeps");
        Assert.True(response.IsSuccessStatusCode);
        
        File.Delete(writeTestPath);
        CSVDatabase<Cheep>.SetPathForTest(sampleDB);
    }
}