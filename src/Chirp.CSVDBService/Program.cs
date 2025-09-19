using Chirp.CLI.Models;
using SimpleDB;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.GetInstance();

app.MapGet("/cheeps", () =>
{
    IEnumerable<Cheep> cheeps = database.Read();
    return Results.Ok(cheeps);
});

app.MapPost("/cheep", (Cheep cheep) =>
{
    try
    {
        database.Store(cheep);
        return Results.Ok($"Uploaded cheep, med message: {cheep.Message} fra author {cheep.Author}");
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.Run();

