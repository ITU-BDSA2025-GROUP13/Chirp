using Microsoft.EntityFrameworkCore;

namespace Chirp.Domain
{
    public interface IChirpDbContext
    {
        /// <summary>
        /// Gets or sets the collection of users in the database.
        /// </summary>
        DbSet<Author> Authors { get; set; }

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