using Chirp.SQLite;
using Chirp.Models;
public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pagenum = 0);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pagenum);
}

public class CheepService : ICheepService
{
    private static readonly IChirpFacade _db = new DBFacade();

    // Limit the amount of Cheeps displayed at any given time. Set to 4 for testing easier purposes
    private int _limit = 32;

    // Returns at most N cheeps from public timeline
    public List<CheepViewModel> GetCheeps(int pagenum)
    {
        IEnumerable<CheepViewModel> cheeps = _db.Read();

        // If collection is empty or null, return empty list.
        if (cheeps == null)
        {
            return new List<CheepViewModel>();
        }

        // Uses list as IEnumerable to not nuke memory usage for large collections.
        try
        {
            return cheeps.Skip(pagenum * _limit).Take(_limit).ToList();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine(e.Message);
            return new List<CheepViewModel>();
        }
    }

    // Returns at most N Cheeps from author
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pagenum)
    {
        IEnumerable<CheepViewModel> cheeps = _db.Read();

        // If collection is empty or null, return empty list.
        if (cheeps == null)
        {
            return new List<CheepViewModel>();
        }

        return cheeps.Where(x => x.Author == author).Skip(pagenum * _limit).Take(_limit).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
