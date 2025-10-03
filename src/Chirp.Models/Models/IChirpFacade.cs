namespace Chirp.Models
{
    public interface IChirpFacade
    {
        /// <summary>
        /// Returns a single page cheeps
        /// </summary>
        /// <param name="pagenum">Which page of cheeps is wished for (the higher the older cheeps)</param>
        /// <returns></returns>
        IEnumerable<Cheep> ReadPage(int pagenum = 0);
        /// <summary>
        /// Returns a single author containing with all the cheeps related to that author
        /// </summary>
        /// <param name="username">The username of the author of the cheeps</param>
        /// <param name="pagenum">Which page of cheeps is wished for (the higher the older cheeps)</param>
        /// <returns></returns>
        Author? ReadPageFromAuthor(string username, int pagenum = 0);
        /// <summary>
        /// Inserts a cheep into the database
        /// </summary>
        void Create(Cheep cheep);
        
        /// <summary>
        /// Inserts an author into the database
        /// </summary>
        void InsertAuthor(Author author);
        
        /// <returns>An author if it exists given the username</returns>
        Author? GetAuthorFromUsername(string username);
        
        
    }
}