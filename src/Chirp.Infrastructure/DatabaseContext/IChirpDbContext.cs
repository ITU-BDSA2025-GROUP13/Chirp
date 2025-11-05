using Chirp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.DatabaseContext
{
    public interface IChirpDbContext
    {

        /// <summary>
        /// Gets or sets the collection of messages in the database.
        /// </summary>
        DbSet<Cheep> Cheeps { get; set; }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}