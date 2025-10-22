using Chirp.Core.Models;

namespace Chirp.Infrastructure.Services

{
    public interface ICheepService
    {
        public List<CheepDTO> GetMainPageCheeps(int pagenum = 0);
        /// <summary>
        /// Returns at most N Cheeps from author. Will create a new author on request if no author exist.
        /// </summary>
        /// <param name="authorName"></param>
        /// <param name="pagenum"></param>
        /// <returns>The cheeps associated with an author, or a emptyList if author doesn't exist</returns>
        public List<CheepDTO> GetCheepsFromAuthorName(string authorName, int pagenum = 0);
        public List<CheepDTO> GetCheepsFromAuthorID(int authorID, int pagenum = 0);
        public List<CheepDTO> GetCheepsFromAuthorEmail(string authorEmail, int pagenum = 0);
        public void PostCheep(String text, int authorID);
    }
}