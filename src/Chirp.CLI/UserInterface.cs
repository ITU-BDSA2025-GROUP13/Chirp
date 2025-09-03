using Chirp.CLI.Models;

static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author} @ {DateTime.UnixEpoch.AddSeconds(cheep.Timestamp + 7200).ToString("MM/dd/yy HH:mm:ss").Replace("-", "/")}: {cheep.Message}");
        }

    }
}
