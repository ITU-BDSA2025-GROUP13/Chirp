using Chirp.CLI.Models;
using SimpleDB;

public class Program
{

    public static void Main(string[] args)
    {

        IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.GetInstance();

        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();


        app.MapPost("/cheep/{userName}/{message}", (string userName, string message) =>
        {
            try
            {
                var cheep = new Cheep(userName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                database.Store(cheep);
                return Results.Ok($"Posted Cheep with message: {message} from {Environment.UserName}");
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });

        app.MapGet("/cheeps", () =>
        {
            try
            {
                var readValues = database.Read();
                return Results.Ok(readValues);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });

        app.MapGet("/cheeps/{numberOfCheeps}", (int numberOfCheeps) =>
        {
            try
            {
                var readValues = database.Read(numberOfCheeps);
                return Results.Ok(readValues);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });

        app.MapGet("/test/", () => { return Results.Ok("Server is up and running!"); });
        app.MapGet("/", () => { return Results.Ok("Greetings, traveler!"); });

        app.Run();
    }
}
