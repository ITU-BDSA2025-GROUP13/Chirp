namespace Chirp.Infrastructure.Services;

public interface IChirpUserService
{
    public void AddFollowerToUser(string userAName, string userBName);
}