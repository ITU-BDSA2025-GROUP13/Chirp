namespace Chirp.Models
{
    public interface IChirpFacade
    {
        IEnumerable<CheepViewModel> Read(string? user = null, int n = 0);
        void Create(CheepViewModel cheep);
    }
}