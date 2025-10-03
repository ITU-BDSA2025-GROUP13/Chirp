using Chirp.SQLite;
using Chirp.Models;
public interface ICheepService
{
    public List<Cheep> GetCheeps(int pagenum = 0);
    /// <summary>
    /// Returns at most N Cheeps from author. Will create a new author on request if no author exist.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="pagenum"></param>
    /// <returns>The cheeps associated with an author, or a emptyList if author doesn't exist</returns>
    public List<Cheep> GetCheepsFromAuthor(string username, int pagenum);
}

public class CheepService : ICheepService
{
    private static readonly IChirpFacade _db = new DBFacade();

    /// Returns at most N cheeps from public timeline
    public List<Cheep> GetCheeps(int pagenum)
    {
        return _db.ReadPage(pagenum).ToList();
    }


    public List<Cheep> GetCheepsFromAuthor(string username, int pagenum)
    {
        Author? author = _db.ReadPageFromAuthor(username, pagenum);
        if (author == null) return new List<Cheep>();
        return author.Cheeps.ToList();
    }
}
