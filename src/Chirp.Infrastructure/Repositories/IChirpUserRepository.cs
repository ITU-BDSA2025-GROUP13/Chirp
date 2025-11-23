using Chirp.Core.Models;

namespace Chirp.Infrastructure.Repositories
{
    public interface IChirpUserRepository
    {
        /// <summary>
        /// Adds userA to include userB in their FollowList
        /// </summary>  
        /// <param name="userA">The follower</param>
        /// <param name="userB">The followed</param>
        /// <returns>Nothing of value</returns>
        Task AddToFollowerList(ChirpUser userA, ChirpUser userB);

        /// <summary>
        /// Remove userB from userA's FollowsList
        /// </summary>
        /// <param name="userA">The follower</param>
        /// <param name="userB">The followed</param>
        /// <returns>Nothing of value</returns>
        Task RemoveFromFollowerList(ChirpUser userA, ChirpUser userB);

        /// <summary>
        /// Returns true if userA has a relation with userB (e.g. Does A follow B?)
        /// </summary>
        /// <param name="userA">The follower</param>
        /// <param name="userB">The followed</param>
        /// <returns>true if A follows B, otherwise false</returns>
        bool ContainsRelation(ChirpUser userA, ChirpUser userB);

        /// <summary>
        /// Get a list of users that the provided user is following.
        /// </summary>
        /// <param name="user">The user whose followed list is requested.</param>
        /// <returns>List of users that the user is following.</returns>
        List<ChirpUser> GetFollowedUsers(ChirpUser user);
    }
}