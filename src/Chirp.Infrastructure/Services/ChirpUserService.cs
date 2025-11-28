using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Services;

public class ChirpUserService(IChirpUserRepository chirpUserRepo, UserManager<ChirpUser> userManager) : IChirpUserService
{
    /// <summary>
    /// Get a list of usernames that the provided user is following.
    /// </summary>
    /// <param name="username">The username of the user whose followed list is requested.</param>
    /// <returns>List of usernames that the user is following.</returns>
    public List<string> GetFollowedUsernames(string username)
    {
        ChirpUser? user = userManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if (user == null)
        {
            Console.WriteLine("User not found during GetFollowedUsernames()");
            return new List<string>();
        }

        List<ChirpUser>? followedUsers = chirpUserRepo.GetFollowedUsers(user);
        bool usedFallback = false;

        if (followedUsers == null || followedUsers.Count == 0)
        {
            followedUsers = chirpUserRepo.GetListOfFollowers(user).GetAwaiter().GetResult();
            usedFallback = true;
        }

        if (followedUsers == null)
        {
            return new List<string>();
        }

        IEnumerable<string> names = followedUsers
            .Select(u => u?.UserName ?? string.Empty)
            .Where(name => !string.Equals(name, user.UserName));

        if (!usedFallback)
        {
            names = names.Where(name => !string.IsNullOrEmpty(name));
        }

        return names.ToList();
    }

    /// <summary>
    /// UserA wants to follow UserB from a Cheep.
    /// Cheeps author is found and then a relation is created.
    /// </summary>
    /// <param name="userAName">Username provided by User.Identity.Name</param>
    /// <param name="userBName">Username provided by CheepDTO.AuthorName</param>
    public void ToggleUserFollowing(String userAName, String userBName)
    {
        ChirpUser? userA = userManager.FindByNameAsync(userAName).GetAwaiter().GetResult();
        ChirpUser? userB = userManager.FindByNameAsync(userBName).GetAwaiter().GetResult();
        if (userA == null || userB == null)
        {
            Console.WriteLine("A user was not found during FollowUserByCheep()");
            return;
        }

        if (chirpUserRepo.ContainsRelation(userA, userB))
        {
            Console.WriteLine($"Remove follower: {userA.UserName} -> {userB.UserName}");
            _ = chirpUserRepo.RemoveFromFollowerList(userA, userB);
        }
        else
        {
            Console.WriteLine($"Add follower: {userA.UserName} -> {userB.UserName}");
            _ = chirpUserRepo.AddToFollowerList(userA, userB);
        }
    }

    /// <summary>
    /// Returns a list of ChirpUsers, which the given user follows.
    /// </summary>
    /// <param name="username">Given user to check following relations</param>
    /// <returns>List of ChirpUsers</returns>
    public async Task<List<ChirpUser>> GetListOfFollowers(string username)
    {
        ChirpUser? user = userManager.FindByNameAsync(username).GetAwaiter().GetResult();
        if (user == null)
        {
            return new List<ChirpUser>();
        }
        return await chirpUserRepo.GetListOfFollowers(user);
    }

    /// <summary>
    /// Anonymizes all personal data for the given user.
    /// </summary>
    /// <param name="username">The username of the user whose data is to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>  
    public async Task ForgetUser(string username)
    {
        ChirpUser? user = await userManager.FindByNameAsync(username);
        if (user == null)
        {
            throw new InvalidOperationException("User not found during ForgetUser()");
        }

        // Remove any external sign-in associations to prevent OAuth re-authentication.
        var externalLogins = await userManager.GetLoginsAsync(user);
        foreach (var login in externalLogins)
        {
            var removeResult = await userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to remove external login '{login.LoginProvider}' for user '{user.Id}': {errors}");
            }
        }   
        
        await chirpUserRepo.ForgetUser(user);        
        await userManager.UpdateAsync(user);
    }
}