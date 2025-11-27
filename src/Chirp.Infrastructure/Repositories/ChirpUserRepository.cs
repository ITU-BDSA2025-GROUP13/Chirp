using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories
{
    public class ChirpUserRepository(IChirpDbContext dbContext) : IChirpUserRepository
    {
        #region INSERT
        #endregion

        #region UPDATE
        public async Task AddToFollowerList(ChirpUser userA, ChirpUser userB)
        {
            await dbContext.ChirpUsers
                .Entry(userA)
                .Collection(u => u.FollowsList)
                .LoadAsync();

            userA.FollowsList.Add(userB);

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromFollowerList(ChirpUser userA, ChirpUser userB)
        {
            await dbContext.ChirpUsers
                .Entry(userA)
                .Collection(u => u.FollowsList)
                .LoadAsync();

            userA.FollowsList.Remove(userB);

            await dbContext.SaveChangesAsync();
        }
        #endregion

        #region GET
        public bool ContainsRelation(ChirpUser userA, ChirpUser userB)
        {
            return dbContext.ChirpUsers
                .Where(u => u.Id == userA.Id)
                .Any(u => u.FollowsList.Any(f => f.Id == userB.Id));
        }

        public List<ChirpUser> GetFollowedUsers(ChirpUser user)
        {
            var trackedUser = dbContext.ChirpUsers
                .Include(u => u.FollowsList)
                .SingleOrDefault(u => u.Id == user.Id);

            return trackedUser?.FollowsList.ToList() ?? new List<ChirpUser>();
        }

        public async Task<List<ChirpUser>> GetListOfFollowers(ChirpUser user)
        {
            await dbContext.ChirpUsers
                .Entry(user)
                .Collection(u => u.FollowsList)
                .LoadAsync();

            // might not be necessary, but keeping it just in case.
            await dbContext.SaveChangesAsync();

            return user.FollowsList;
        }
        #endregion
    }
}
