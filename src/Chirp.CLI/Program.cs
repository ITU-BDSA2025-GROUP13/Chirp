using System.CommandLine;
using Chirp.CLI.Models;
using SimpleDB;

class Program
{
    static int Main(string[] args)
    {
        IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();

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

        rootCommand.SetAction(parseResult =>
        {
            if (parseResult.Errors.Count == 0)
            {
                int readCount = parseResult.GetValue(readOption);
                if (readCount > 0)
                {
                    var cheeps = database.Read(readCount); // TODO: error on negative count
                    readCheeps(cheeps);
                }

                if (parseResult.GetValue(readAllOption))
                {
                    var cheeps = database.Read();
                    readCheeps(cheeps);
                }

                var message = parseResult.GetValue(chirpOption);
                if (message is not null)
                {
                    Cheep cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    database.Store(cheep);
                }
            }
        });

        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }

    static void readCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author} @ {DateTime.UnixEpoch.AddSeconds(cheep.Timestamp + 7200).ToString("MM/dd/yy HH:mm:ss").Replace("-", "/")}: {cheep.Message}");
        }

    }
}
