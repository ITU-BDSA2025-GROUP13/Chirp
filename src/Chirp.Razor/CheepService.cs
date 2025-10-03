using Chirp.Domain;
using Chirp.Infrastructure;

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

    private readonly ICheepRepository _repository;

    public CheepService()
    {
        Database database = new Database();
        _repository = new CheepRepository(database);
    }

    /// Returns at most N cheeps from public timeline
    public List<Cheep> GetCheeps(int page = 0)
    {
        return _repository.ReadPageAsync(page).GetAwaiter().GetResult().ToList();
    }

    public List<Cheep> GetCheepsFromAuthor(string author, int page = 0)
    {
        var result = _repository.ReadPageFromAuthorAsync(author, page).GetAwaiter().GetResult();
        return result == null ? new List<Cheep>() : result.Cheeps.ToList<Cheep>();
    }

    public Author? GetAuthorFromUsername(string username)
    {
        return _repository.GetAuthorFromUsername(username);
    }

    public void PostCheep(Cheep cheep)
    {
        _repository.PostAsync(cheep).GetAwaiter().GetResult();
    }
}
