using Chirp.Core.Models;

namespace Chirp.Infrastructure.Services;

public interface IChirpUserService
{   
    /// <summary>
    /// UserA wants to follow UserB from a Cheep.
    /// Cheeps author is found and then a relation is created.
    /// </summary>
    /// <param name="userAName">Username provided by User.Identity.Name</param>
    /// <param name="userBName">Username provided by CheepDTO.AuthorName</param>
    public void ToggleUserFollowing(string userAName, string userBName);

    /// <summary>
    /// Get a list of usernames that the provided user is following.
    /// </summary>
    /// <param name="username">The username of the user whose followed list is requested.</param>
    /// <returns>List of usernames that the user is following.</returns>
    public List<string> GetFollowedUsernames(string username);

    /// <summary>
    /// Returns a list of ChirpUsers, which the given user follows.
    /// </summary>
    /// <param name="username">Given user to check following relations</param>
    /// <returns>List of ChirpUsers</returns>
    public Task<List<ChirpUser>> GetListOfFollowers(string username);

    /// <summary>
    /// Anonymizes all personal data for the given user.
    /// </summary>
    /// <param name="username">The username of the user whose data is to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>  
    public Task ForgetUser(string username);
}