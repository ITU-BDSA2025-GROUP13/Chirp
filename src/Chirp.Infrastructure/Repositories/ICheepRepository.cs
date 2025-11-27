using Chirp.Core.Models;

namespace Chirp.Infrastructure.Repositories
{
    public interface ICheepRepository
    {
        /// <summary>
        /// Retrieves a paginated list of cheeps ordered by publication date (newest first).
        /// </summary>
        /// <param name="pagenum">The one-based page number to retrieve. Default is 1.</param>
        /// <returns>A task containing an enumerable collection of cheeps for the specified page.</returns>
        Task<IEnumerable<Cheep>> GetMainPage(int pagenum = 1);

        /// <summary>
        /// Retrieves an author with a paginated list of their cheeps ordered by publication date (newest first).
        /// </summary>
        /// <param name="author">The author of the cheeps to retrieve.</param>
        /// <param name="pagenum">The one-based page number to retrieve. Default is 1.</param>
        /// <returns>A task containing the author with their cheeps, or null if the author is not found.</returns>
        Task<IEnumerable<Cheep>> GetAuthorPage(ChirpUser author, int pagenum = 1);

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

        /// <summary>
        /// Get users own private timeline, which contains their own cheeps and cheeps of users they follow
        /// </summary>
        /// <param name="user">Given ChirpUser to get private timeline for</param>
        /// <param name="pagenum">The one-based page to retrieve.</param>
        /// <returns>An IEnumerable of Cheeps</returns>
        Task<IEnumerable<Cheep>> GetPrivateTimelineCheeps(ChirpUser user, int pagenum = 1);

        /// <summary>
        /// Edit a cheep matching the id
        /// </summary>
        /// <param name="cheepId">The cheepId of the post to edit.</param>
        /// <param name="newTest">The text to replace the old text with.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the cheepId does not match any existing cheep in the database.</exception>
        Task EditCheepById(int cheepId, string newText);

        /// <summary>
        /// Retrieves a collection of all cheeps posted by a given author ordered by publication date (newest first).
        /// </summary>
        /// <param name="author">The author of the cheeps to retrieve.</param>
        /// <returns>A task containing the author with their cheeps, or null if the author is not found.</returns>
        Task<IEnumerable<Cheep>> GetAllAuthorCheeps(ChirpUser author);

        /// <summary>
        /// Likes a cheep.
        /// </summary>
        Task LikeCheep(int cheepId, string userId);

        /// <summary>
        /// Removes like from cheep
        /// </summary>
        Task UnLikeCheep(int cheepId, string userId);
    }
}
