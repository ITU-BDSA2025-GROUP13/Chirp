using Chirp.Domain;
using Chirp.Infrastructure;

public interface ICheepService
{
    public List<Cheep> GetCheeps(int pagenum = 0);
    public List<Cheep> GetCheepsFromAuthor(string author, int pagenum);
}

public class CheepService : ICheepService
{

    private readonly ICheepRepository _repository;

    public CheepService()
    {
        Database database = new Database();
        _repository = new CheepRepository(database);
    }

    public List<Cheep> GetCheeps(int page = 0)
    {
        return _repository.ReadPageAsync(page).GetAwaiter().GetResult().ToList();
    }

    public List<Cheep> GetCheepsFromAuthor(string author, int page = 0)
    {
        return _repository.ReadPageFromAuthorAsync(author, page).GetAwaiter().GetResult().ToList();
    }

    public void PostCheep(Cheep cheep)
    {
        _repository.PostAsync(cheep).GetAwaiter().GetResult();
    }
}
