using Chirp.Models;

namespace Chirp.Repository
{
    public interface ICheepRepository
    {
        /// <summary>
        /// Returns a single page cheeps
        /// </summary>
        /// <param name="pagenum">Which page of cheeps is wished for (the higher the older cheeps)</param>
        /// <returns></returns>
        IEnumerable<Cheep> ReadPage(int pagenum = 0);
        /// <summary>
        /// Returns a single page of cheeps from a given author
        /// </summary>
        /// <param name="username">The author of the cheeps</param>
        /// <param name="pagenum">Which page of cheeps is wished for (the higher the older cheeps)</param>
        /// <returns></returns>
        IEnumerable<Cheep> ReadPageFromAuthor(string username, int pagenum = 0);
        /// <summary>
        /// Inserts a cheep into the database
        /// </summary>
        void Create(Cheep cheep);
    }
}
