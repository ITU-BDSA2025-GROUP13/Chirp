using Chirp.CLI.Models;
using SimpleDB;


public class Server
{
    public static void Main(String[] args)
    {
        IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.GetInstance();

        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/cheeps", () =>
        {
            try
            {
                IEnumerable<Cheep> cheeps = database.Read();
                return Results.Ok(cheeps);
            }
            catch (Exception e)
            {
                return Results.Problem(e.Message);
            }
        });

        app.MapGet("/cheep/{message}", (String message) =>
        {
            try
            {
                var cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                database.Store(cheep);
                return Results.Ok($"Posted Cheep with message: {message} from {Environment.UserName}");
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });

        app.Run();
    }
}
