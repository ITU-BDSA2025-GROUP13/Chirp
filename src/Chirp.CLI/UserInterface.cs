using Chirp.CLI;
using Chirp.CLI.Models;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            long timeZoneAdjustedTimeStamp = TimeConverter.UCTToUCT2(cheep.Timestamp);
            string readableTime = TimeConverter.ToReadable(timeZoneAdjustedTimeStamp);
            Console.WriteLine($"{cheep.Author} @ {readableTime}: {cheep.Message}");
        }

    }
}
