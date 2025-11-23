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

        List<ChirpUser> followedUsers = chirpUserRepo.GetFollowedUsers(user);
        return followedUsers.Select(u => u.UserName ?? "").Where(name => !string.IsNullOrEmpty(name)).ToList();
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
}