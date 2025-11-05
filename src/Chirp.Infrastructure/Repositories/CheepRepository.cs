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
            if (cheep.AuthorId <= 0) throw new ArgumentNullException($"Invalid ID {cheep.AuthorId}");
            cheep.Author = null; //Prevents EFCore from auto-inserting a new author

            dbContext.Cheeps.Add(cheep);
            await dbContext.SaveChangesAsync();
        }
        #endregion

        #region UPDATE
        #endregion

        #region GET
        public async Task<IEnumerable<Cheep>> GetMainPage(int pagenum = 0)
        {
            return await dbContext.Cheeps
                .Include(m => m.Author) //Joins Author's 
                .OrderByDescending(m => m.TimeStamp)
                .Skip(pagenum * _readLimit)
                .Take(_readLimit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cheep>> GetAuthorPage(ChirpUser author, int pagenum = 0)
        {
            return await dbContext.Cheeps
                .Where(m => m.AuthorId == author.Id) //Joins Author's 
                .OrderByDescending(m => m.TimeStamp)
                .Skip(pagenum * _readLimit)
                .Take(_readLimit)
                .ToListAsync();
        }
        #endregion
    }
}
