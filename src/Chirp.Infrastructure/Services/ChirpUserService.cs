using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Services;

public class ChirpUserService(IChirpUserRepository chirpUserRepo, UserManager<ChirpUser> userManager) : IChirpUserService
{
    /// <summary>
    /// UserA wants to follow UserB from a Cheep.
    /// Cheeps author is found and then a relation is created.
    /// </summary>
    /// <param name="userAName">Username provided by User.Identity.Name</param>
    /// <param name="userBName">Username provided by CheepDTO.AuthorName</param>
    public void AddFollowerToUser(String userAName, String userBName)
    {
        ChirpUser? userA = userManager.FindByNameAsync(userAName).GetAwaiter().GetResult();
        ChirpUser? userB = userManager.FindByNameAsync(userBName).GetAwaiter().GetResult();
        if (userA == null || userB == null)
        {
            Console.WriteLine("A user was not found during FollowUserByCheep()");
            return;
        }
        chirpUserRepo.UpdateFollowerList(userA, userB);
    }
}