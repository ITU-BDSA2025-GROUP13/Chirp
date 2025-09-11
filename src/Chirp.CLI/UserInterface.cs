using Chirp.CLI;
using Chirp.CLI.Models;

static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            long uct2UnixTimeStamp = TimeConverter.UCTToUCT2(cheep.Timestamp);
            string readableTime = TimeConverter.ToReadable(uct2UnixTimeStamp);
            Console.WriteLine($"{cheep.Author} @ {readableTime}: {cheep.Message}");
        }

    }
}
