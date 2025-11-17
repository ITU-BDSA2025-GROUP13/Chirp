using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chirp.Infrastructure.Services;

public class ChirpUserService(IChirpUserRepository chirpUserRepo, UserManager<ChirpUser> userManager) : IChirpUserService
{
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
            Console.WriteLine($"Add follower: {userA.UserName} -> {userB.UserName}");
            _ = chirpUserRepo.RemoveFromFollowerList(userA, userB);
        }
        else
        {
            Console.WriteLine($"Remove follower: {userA.UserName} -> {userB.UserName}");
            _ = chirpUserRepo.AddToFollowerList(userA, userB);
        }
    }
}