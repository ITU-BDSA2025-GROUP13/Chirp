using Chirp.CLI.Models;
using SimpleDB;

IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.GetInstance();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/cheep/{message}", (String message) =>
{
    var cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
    database.Store(cheep)
});

app.MapGet("/cheeps", () => database.Read());


app.Run();
