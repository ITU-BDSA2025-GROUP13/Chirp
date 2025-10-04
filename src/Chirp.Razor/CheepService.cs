using Chirp.Domain;
using Chirp.Infrastructure;
public interface ICheepService
{
    public List<Cheep> GetCheeps(int pagenum = 0);
    public List<Cheep> GetCheepsFromAuthor(string author, int pagenum);
}

public class CheepService : ICheepService
{

    private static readonly CheepController _cheepController = new CheepController();

    /// Returns at most N cheeps from public timeline
    public List<Cheep> GetCheeps(int pagenum)
    {
        return _cheepController.GetCheeps(pagenum);
    }

    /// Returns at most N Cheeps from author
    public List<Cheep> GetCheepsFromAuthor(string author, int pagenum)
    {
        return _cheepController.GetCheepsFromAuthor(author, pagenum);
    }
}
