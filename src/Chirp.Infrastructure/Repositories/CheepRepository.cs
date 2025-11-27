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
            int skip = pagenum < 1 ? 0 : (pagenum - 1) * _readLimit;

            List<Cheep> cheeps = await dbContext.Cheeps
                .AsNoTracking()
                .Where(c => c.ParentCheep == null)
                .Include(c => c.Author)
                .Include(c => c.UsersWhoLiked)
                .Include(c => c.ParentCheep)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(skip)
                .Take(_readLimit)
                .ToListAsync();

            await PopulateRepliesRecursiveAsync(cheeps);

            return cheeps;
        }

        public async Task<IEnumerable<Cheep>> GetPrivateTimelineCheeps(ChirpUser user, int pagenum = 1)
        {
<<<<<<< HEAD
            List<ChirpUser> followers = await GetListOfFollowers(user);
            followers.Add(user);

            int skip = pagenum < 1 ? 0 : (pagenum - 1) * _readLimit;

            List<Cheep> cheeps = await dbContext.Cheeps
                .AsNoTracking()
                .Where(c => followers.Contains(c.Author) && c.ParentCheep == null)
                .Include(c => c.Author)
                .Include(c => c.Author)
                .Include(c => c.UsersWhoLiked)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(skip)
=======
            dbContext.ChirpUsers
                .Entry(user)
                .Collection(u => u.FollowsList)
                .Load();
                
            List<ChirpUser> followsList = user.FollowsList;
            followsList.Add(user); // add own cheeps to private timeline

            return await dbContext.Cheeps
                .Include(m => m.Author)
                .Where(m => followsList.Contains(m.Author))
                .Include(m => m.Author) //Joins Author's 
                .Include(m => m.UsersWhoLiked) //Joins users who liked
                .OrderByDescending(m => m.TimeStamp)
                .Skip(pagenum * _readLimit)
>>>>>>> f608645 (refactor: move user related methods from cheep service/repo to user service/repo)
                .Take(_readLimit)
                .ToListAsync();

            await PopulateRepliesRecursiveAsync(cheeps);

            return cheeps;
        }

<<<<<<< HEAD
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

        public async Task<IEnumerable<Cheep>> GetAuthorPage(ChirpUser author, int pagenum = 1)
=======
        public async Task<IEnumerable<Cheep>> GetAuthorPage(ChirpUser author, int pagenum = 0)
>>>>>>> f608645 (refactor: move user related methods from cheep service/repo to user service/repo)
        {
            int skip = pagenum < 1 ? 0 : (pagenum - 1) * _readLimit;

            List<Cheep> cheeps = await dbContext.Cheeps
                .AsNoTracking()
                .Where(c => c.AuthorId == author.Id && c.ParentCheep == null)
                .Include(c => c.Author)
                .Include(c => c.UsersWhoLiked)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(skip)
                .Take(_readLimit)
                .ToListAsync();

            await PopulateRepliesRecursiveAsync(cheeps);

            return cheeps;
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
                .AsNoTracking()
                .Where(m => m.AuthorId == author.Id)
                .Include(m => m.Author)
                .OrderByDescending(m => m.TimeStamp)
                .ToListAsync();
        }
        #endregion

        private async Task PopulateRepliesRecursiveAsync(List<Cheep> rootCheeps)
        {
            if (!rootCheeps.Any()) return;

            Dictionary<int, Cheep> lookup = new Dictionary<int, Cheep>();
            List<Cheep> parentsToProcess = new List<Cheep>();
            foreach (Cheep root in rootCheeps)
            {
                lookup[root.CheepId] = root;
                root.Replies ??= new List<Cheep>();
                parentsToProcess.Add(root);
            }

            while (parentsToProcess.Any())
            {
                List<int> parentIds = parentsToProcess.Select(c => c.CheepId).ToList();
                parentsToProcess = new List<Cheep>();

                List<Cheep> replies = await dbContext.Cheeps
                    .Where(c => c.ParentCheep != null && parentIds.Contains(c.ParentCheep.CheepId))
                    .AsNoTracking()
                    .Include(c => c.Author)
                    .Include(c => c.ParentCheep)
                    .Include(c => c.UsersWhoLiked)
                    .OrderBy(c => c.TimeStamp)
                    .ToListAsync();

                if (!replies.Any())
                {
                    break;
                }

                foreach (Cheep reply in replies)
                {
                    if (reply.ParentCheep == null)
                    {
                        continue;
                    }

                    int parentId = reply.ParentCheep.CheepId;
                    if (!lookup.TryGetValue(parentId, out Cheep? parent))
                    {
                        continue;
                    }

                    if (parent.Replies.All(r => r.CheepId != reply.CheepId))
                    {
                        parent.Replies.Add(reply);
                    }

                    if (!lookup.ContainsKey(reply.CheepId))
                    {
                        lookup[reply.CheepId] = reply;
                        reply.Replies = new List<Cheep>();
                        parentsToProcess.Add(reply);
                    }
                }
            }
        }
    }
}
