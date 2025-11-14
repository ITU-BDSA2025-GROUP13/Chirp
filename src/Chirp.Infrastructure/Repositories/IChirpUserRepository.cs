using Chirp.Core.Models;

namespace Chirp.Infrastructure.Repositories
{
    public interface IChirpUserRepository
    {
        /// <summary>
        /// Updates userA to include userB in their FollowList
        /// </summary>  
        /// <param name="userA"></param>
        /// <param name="userB"></param>
        /// <returns></returns>
        Task UpdateFollowerList(ChirpUser userA, ChirpUser userB);
    }
}
