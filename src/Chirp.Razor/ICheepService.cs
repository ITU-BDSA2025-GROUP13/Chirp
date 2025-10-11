namespace Chirp.Razor
{
    public interface ICheepService
    {
        public List<CheepDTO> GetCheeps(int pagenum = 0);
        /// <summary>
        /// Returns at most N Cheeps from author. Will create a new author on request if no author exist.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pagenum"></param>
        /// <returns>The cheeps associated with an author, or a emptyList if author doesn't exist</returns>
        public List<CheepDTO> GetCheepsFromAuthor(string username, int pagenum = 0);
        public void PostCheep(String text, int authorID);
    }
}