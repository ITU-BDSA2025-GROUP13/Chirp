using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing Cheep entities and their associated authors in the database.
    /// Implements the ICheepRepository interface to provide data access operations.
    /// </summary>
    /// <param name="dbContext">The database context used for data access operations.</param>
    public class CheepRepository(IChirpDbContext dbContext) : ICheepRepository
    {
        private readonly int _readLimit = 32;

        #region INSERT
        public async Task InsertCheep(Cheep cheep)
        {
            if (cheep.AuthorId == null) throw new ArgumentNullException($"AuthorID is null");
            dbContext.Cheeps.Add(cheep);
            await dbContext.SaveChangesAsync();
        }
        #endregion

        #region LIKE
        public async Task LikeCheep(int cheepId, string userId)
        {
            Cheep? cheep = await dbContext.Cheeps
                    .Include(m => m.UsersWhoLiked)
                    .FirstOrDefaultAsync(m => m.CheepId == cheepId);

            ChirpUser? user = await dbContext.ChirpUsers
                    .Include(u => u.LikedCheeps)
                    .FirstOrDefaultAsync(m => m.Id == userId);

            if (cheep == null)
            {
                throw new DbUpdateException("Failed to find cheep");
            }

            if (user == null)
            {
                throw new DbUpdateException("Failed to find user");
            }

            if (cheep.AuthorId == null) throw new ArgumentNullException($"AuthorID is null");
            if (user.Id == null) throw new ArgumentNullException($"ID is null");

            cheep.UsersWhoLiked.Add(user);
            user.LikedCheeps.Add(cheep);

            await dbContext.SaveChangesAsync();
        }

        public async Task UnLikeCheep(int cheepId, string userId)
        {
            Cheep? cheep = await dbContext.Cheeps
                    .Include(m => m.UsersWhoLiked)
                    .FirstOrDefaultAsync(m => m.CheepId == cheepId);

            ChirpUser? user = await dbContext.ChirpUsers
                    .Include(u => u.LikedCheeps)
                    .FirstOrDefaultAsync(m => m.Id == userId);

            if (cheep == null)
            {
                throw new DbUpdateException("Failed to find cheep");
            }

            if (user == null)
            {
                throw new DbUpdateException("Failed to find user");
            }

            if (cheep.AuthorId == null) throw new ArgumentNullException($"AuthorID is null");
            if (user.Id == null) throw new ArgumentNullException($"ID is null");

            cheep.UsersWhoLiked.Remove(user);
            user.LikedCheeps.Remove(cheep);

            await dbContext.SaveChangesAsync();
        }
        #endregion

        #region UPDATE
        public async Task EditCheepById(int cheepId, string newText)
        {
            Cheep? cheep = await dbContext.Cheeps
                .FirstOrDefaultAsync(m => m.CheepId == cheepId);

            if (cheep == null)
            {
                throw new DbUpdateException("Failed to find cheep");
            }

            cheep.Text = newText;
            dbContext.Cheeps.Update(cheep);
            await dbContext.SaveChangesAsync();
        }
        #endregion

        #region GET
        public async Task<IEnumerable<Cheep>> GetMainPage(int pagenum = 1)
        {
            return await dbContext.Cheeps
                .Include(c => c.Author)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .Include(c => c.UsersWhoLiked)
                .Include(c => c.Replies)
                .ThenInclude(r => r.UsersWhoLiked)
                .Include(c => c.ParentCheep)
                .OrderByDescending(c => c.TimeStamp)
                .Skip((pagenum - 1) * _readLimit)
                .Take(_readLimit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cheep>> GetPrivateTimelineCheeps(ChirpUser user, int pagenum = 0)
        {
            List<ChirpUser> fList = await GetListOfFollowers(user);
            fList.Add(user); // add own cheeps to private timeline

            return await dbContext.Cheeps
                .Include(m => m.Author)
                .Where(m => fList.Contains(m.Author))
                .Include(m => m.Author) //Joins Author's 
                .Include(m => m.UsersWhoLiked) //Joins users who liked
                .OrderByDescending(m => m.TimeStamp)
                .Skip(pagenum * _readLimit)
                .Take(_readLimit)
                .ToListAsync();
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

        public async Task<IEnumerable<Cheep>> GetAuthorPage(ChirpUser author, int pagenum = 0)
        {
            return await dbContext.Cheeps
                .Where(c => c.AuthorId == author.Id)
                .Include(c => c.Replies)
                .ThenInclude(r => r.Author)
                .Include(c => c.ParentCheep)
                .OrderByDescending(c => c.TimeStamp)
                .Include(m => m.UsersWhoLiked) //Joins users who liked
                .Where(m => m.AuthorId == author.Id)
                .OrderByDescending(m => m.TimeStamp)
                .Skip(pagenum * _readLimit)
                .Take(_readLimit)
                .ToListAsync();
        }

        public Task DeleteCheep(Cheep cheep)
        {
            dbContext.Cheeps.Attach(cheep);
            dbContext.Cheeps.Remove(cheep);
            return dbContext.SaveChangesAsync();
        }

        public Task<Cheep?> GetCheepById(int cheepID)
        {
            return dbContext.Cheeps
                .Include(c => c.Author)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Author)
                .Include(c => c.ParentCheep)
                .FirstOrDefaultAsync(c => c.CheepId == cheepID);
        }

        public async Task<IEnumerable<Cheep>> GetAllAuthorCheeps(ChirpUser author)
        {
            return await dbContext.Cheeps
                .Where(m => m.AuthorId == author.Id)
                .OrderByDescending(m => m.TimeStamp)
                .ToListAsync();
        }
        #endregion
    }
}
