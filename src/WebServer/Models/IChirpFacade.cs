namespace WebServer.Models
{
    public interface IChirpFacade
    {
        void Read(int n = 0);
        IEnumerable<CheepViewModel> ReadFromAuthor(string user, int n = 0);
        void Post(CheepViewModel cheep);
    }
}
