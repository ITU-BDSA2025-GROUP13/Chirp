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

        public async Task UpdateFollowerList(ChirpUser userA, ChirpUser userB)
        {
            userA.FollowsList.Add(userB);
            userB.FollowedByList.Add(userA);
            await dbContext.SaveChangesAsync();
        }
        
        public async Task RemoveFromFollowerList(ChirpUser userA, ChirpUser userB)
        {
            //await dbContext.ChirpUsers.Entry(userA).Collection(u => u.FollowsList).LoadAsync();
            userA.FollowsList.Remove(userB);
            userB.FollowedByList.Remove(userA);
            await dbContext.SaveChangesAsync();
        }
        
        #endregion

        #region GET

        public bool ContainsRelation(ChirpUser userA, ChirpUser userB)
        {
            if (userA.FollowsList.Contains(userB))
            {
                
            }
        }
        
        #endregion
    }
}
