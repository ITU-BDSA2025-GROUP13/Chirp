using Chirp.Core.Models;

namespace Chirp.Infrastructure.Repositories
{
    public interface ICheepRepository
    {
        /// <summary>
        /// Retrieves a paginated list of cheeps ordered by publication date (newest first).
        /// </summary>
        /// <param name="pagenum">The zero-based page number to retrieve. Default is 0.</param>
        /// <returns>A task containing an enumerable collection of cheeps for the specified page.</returns>
        Task<IEnumerable<Cheep>> GetMainPage(int pagenum = 0);
        /// <summary>
        /// Retrieves an author with a paginated list of their cheeps ordered by publication date (newest first).
        /// </summary>
        /// <param name="author">The author of the cheeps to retrieve.</param>
        /// <param name="pagenum">The zero-based page number to retrieve. Default is 0.</param>
        /// <returns>A task containing the author with their cheeps, or null if the author is not found.</returns>
        Task<IEnumerable<Cheep>> GetAuthorPage(ChirpUser author, int pagenum = 0);
        /// <summary>
        /// Posts a new cheep (message) to the database asynchronously.
        /// </summary>
        /// <param name="cheep">The cheep to post.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the author is not found in the database.</exception>
        Task InsertCheep(Cheep cheep);

        /// <summary>
        /// Deletes a cheep from the database asynchronously.
        /// </summary>
        /// <param name="cheep"></param>
        /// <returns></returns>
        Task DeleteCheep(Cheep cheep);

        /// <summary>
        /// Retrieves a cheep by its ID asynchronously.
        /// </summary>
        /// <param name="cheepID">The ID of the cheep to retrieve.</param>
        /// <returns>A task containing the cheep if found, or null if not found.</returns>
        Task<Cheep?> GetCheepById(int cheepID);
    }
}
