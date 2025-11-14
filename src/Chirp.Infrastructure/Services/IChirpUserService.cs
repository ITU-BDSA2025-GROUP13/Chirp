namespace Chirp.Infrastructure.Services;

public interface IChirpUserService
{
    public void ToggleUserFollowing(string userAName, string userBName);
}