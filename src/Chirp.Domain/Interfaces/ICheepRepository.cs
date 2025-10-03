namespace Chirp.Domain
{
    public interface ICheepRepository
    {
        /// <summary>
        /// Returns a single page cheeps
        /// </summary>
        /// <param name="pagenum">Which page of cheeps is wished for (the higher the older cheeps)</param>
        /// <returns></returns>
        Task<IEnumerable<Cheep>> ReadPageAsync(int pagenum = 0);
        /// <summary>
        /// Returns a single author containing all the cheeps related to that author
        /// </summary>
        /// <param name="username">The username of the author of the cheeps</param>
        /// <param name="pagenum">Which page of cheeps is wished for (the higher the older cheeps)</param>
        /// <returns></returns>
        Task<Author?> ReadPageFromAuthorAsync(string username, int pagenum = 0);
        /// <summary>
        /// Inserts a cheep into the database
        /// </summary>
        Task PostAsync(Cheep cheep);
        /// <summary>
        /// Inserts an author into the database
        /// </summary>
        void InsertAuthor(Author author);
        /// <returns>An author if it exists given the username</returns>
        Author? GetAuthorFromUsername(string username);
    }
}
