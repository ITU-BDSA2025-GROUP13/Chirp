public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pagenum = 0);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pagenum);
}

public class CheepService : ICheepService
{
    // Limit the amount of Cheeps displayed at any given time. Set to 4 for testing easier purposes
    private int _limit = 4;
    
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
        {
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Helge", "Hello, more BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Helge", "Hello, even more BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Helge", "Hello, to all BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Helge", "Hello, but this is the last time!", UnixTimeStampToDateTimeString(1690892305)),
            new CheepViewModel("Rasmus1", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
            new CheepViewModel("Rasmus2", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
            new CheepViewModel("Rasmus3", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
            new CheepViewModel("Rasmus3", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
            new CheepViewModel("Rasmus4", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
            new CheepViewModel("Rasmus4", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895309)),
            new CheepViewModel("Rasmus5", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895334)),
            new CheepViewModel("Rasmus5", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690896308)),
            new CheepViewModel("Rasmus5", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690896358)),
            new CheepViewModel("NextPageMus", "I should be on the last page!.", UnixTimeStampToDateTimeString(1690995408))
        };
    
    // Returns at most N cheeps from public timeline
    public List<CheepViewModel> GetCheeps(int pagenum)
    {
        // If collection is empty or null, return empty list.
        if (_cheeps.Count == 0 || _cheeps == null){ return new List<CheepViewModel>(); }
        
        // Uses list as IEnumerable to not nuke memory usage for large collections.
        try
        {
            return _cheeps.Skip(pagenum * _limit).Take(_limit).ToList();
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
        // If collection is empty or null, return empty list.
        if (_cheeps.Count == 0 || _cheeps == null){ return new List<CheepViewModel>(); }
     
        return _cheeps.Where(x => x.Author == author).Skip(pagenum * _limit).Take(_limit).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
