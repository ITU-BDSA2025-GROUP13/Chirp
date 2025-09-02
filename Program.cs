using Chirp.CLI;
using Chirp.CLI.Models;

const string CSV = "assets/chirp_cli_db.csv";

CsvManager csvm = new CsvManager(CSV);

if (args[0] == "read")
{
    IEnumerable<Cheep> cheeps = csvm.GetCheeps();
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
    csvm.WriteCheep(cheep);
}
