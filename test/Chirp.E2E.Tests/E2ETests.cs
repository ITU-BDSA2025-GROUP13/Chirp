using Chirp.CLI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using SimpleDB;


public class E2ETests
{
    private readonly HttpClient client;
    private readonly string sampleDB = "../../../assets/testSample.csv";
    
    public E2ETests()
    {
        client = new WebApplicationFactory<WebServer.Program>().CreateClient();
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