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

    /// Returns at most N cheeps from public timeline
    public List<CheepViewModel> GetCheeps(int pagenum)
    {
        return _db.ReadPage(pagenum).ToList();
    }

    /// Returns at most N Cheeps from author
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pagenum)
    {
        return _db.ReadPageFromAuthor(author, pagenum).ToList();
    }
}
