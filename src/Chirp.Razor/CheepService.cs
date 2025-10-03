using Chirp.Models;
using Chirp.Repository;
using Chirp.DataBase;
public interface ICheepService
{
    public List<Cheep> GetCheeps(int pagenum = 0);
    public List<Cheep> GetCheepsFromAuthor(string author, int pagenum);
}

public class CheepService : ICheepService
{

    private static readonly ICheepRepository _cheepRepository = new CheepRepository(new DB());

    /// Returns at most N cheeps from public timeline
    public List<Cheep> GetCheeps(int pagenum)
    {
        return _cheepRepository.ReadPage(pagenum).ToList();
    }

    /// Returns at most N Cheeps from author
    public List<Cheep> GetCheepsFromAuthor(string author, int pagenum)
    {
        return _cheepRepository.ReadPageFromAuthor(author, pagenum).ToList();
    }
}
