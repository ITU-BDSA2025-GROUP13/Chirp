namespace Chirp.Models
{
    public interface IChirpFacade
    {
        IEnumerable<CheepViewModel> Read(int n = 0);
        IEnumerable<CheepViewModel> ReadFromAuthor(string user, int n = 0);
        void Post(CheepViewModel cheep);
    }
}
