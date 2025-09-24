using System.CommandLine;
using Chirp.CLI.Models;
using SimpleDB;
using System.Net.Http.Json;


public class Program
{
    static async Task<int> Main(string[] args)
    {
        var baseURL = "https://bdsagroup13chirpremotedb.azurewebsites.net";
        using HttpClient client = new();
        client.BaseAddress = new Uri(baseURL);

        IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.GetInstance();

        RootCommand rootCommand = new("Chirp: read or post cheeps");

        Option<int> readOption = new("read")
        {
            Description = "Read the number cheeps supplied on the commandline",
        };
        rootCommand.Options.Add(readOption);

        Option<bool> readAllOption = new("read-all")
        {
            Description = "Read all the cheeps",
        };
        rootCommand.Options.Add(readAllOption);

        Option<string> chirpOption = new("cheep")
        {
            Description = "Post a new cheep"
        };
        rootCommand.Options.Add(chirpOption);

        rootCommand.SetAction(async parseResult =>
        {
            if (parseResult.Errors.Count == 0)
            {
                int readCount = parseResult.GetValue(readOption);
                if (readCount > 0)
                {
                    var cheeps = await client.GetFromJsonAsync<IEnumerable<Cheep>>("/cheeps/" + readCount);
                    if (cheeps != null)
                        UserInterface.PrintCheeps(cheeps);
                }

                if (parseResult.GetValue(readAllOption))
                {
                    var cheeps = await client.GetFromJsonAsync<IEnumerable<Cheep>>("/cheeps");
                    if (cheeps != null)
                        UserInterface.PrintCheeps(cheeps);
                }

                var message = parseResult.GetValue(chirpOption);
                if (message is not null)
                {
                    var response = await client.PostAsync("/cheep/" + Environment.UserName + "/" + message, null);
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                }
            }
        });

        ParseResult parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
}
