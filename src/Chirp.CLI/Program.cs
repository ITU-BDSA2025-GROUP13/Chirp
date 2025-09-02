using Chirp.CLI;
using Chirp.CLI.Models;
using Microsoft.VisualBasic.CompilerServices;
using SimpleDB;

IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();

if (args[0] == "read") {
    //Includes limit if one is given, else return all cheeps
    var cheeps = args.Length > 1 ? database.Read(int.Parse(args[1])) : database.Read();
    
    foreach (var cheep in cheeps)
    {
        // Read record and parse into desired format. Adds 2 hours in seconds to compensate for timezones.
        Console.WriteLine($"{cheep.Author} @ {DateTime.UnixEpoch.AddSeconds(cheep.Timestamp + 7200).ToString("MM/dd/yy HH:mm:ss").Replace("-", "/")}: {cheep.Message}");
    }
}
else if (args[0] == "cheep")
{
    // Get metadata and cheep, format and save
    Cheep cheep = new Cheep(Environment.UserName, args[1], DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    database.Store(cheep);
}
