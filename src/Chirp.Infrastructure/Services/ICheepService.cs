using Chirp.Core.Models;

namespace Chirp.Infrastructure.Services
{
    public interface ICheepService
    {
        /// <summary>
        /// Returns at the specified page of cheepDTOs.
        /// </summary>
        /// <param name="pagenum"></param>
        /// <returns></returns>
        public List<CheepDTO> GetMainPageCheeps(int pagenum = 0);
        /// <summary>
        /// Returns at most N Cheeps from author. Will create a new author on request if no author exist.
        /// </summary>
        /// <param name="authorName"></param>
        /// <param name="pagenum"></param>
        /// <returns>The cheeps associated with an author, or a emptyList if author doesn't exist</returns>
        public List<CheepDTO> GetCheepsFromAuthorName(string authorName, int pagenum = 0);

        public List<CheepDTO> GetCheepsFromAuthorID(string authorID, int pagenum = 0);
        public List<CheepDTO> GetCheepsFromAuthorEmail(string authorEmail, int pagenum = 0);
        public void PostCheep(string text, string authorID);
        public void DeleteCheep(int cheepID);

        /// <summary>
        /// Replies to a Cheep
        /// </summary>
        /// <param name="cheepID">The cheep that is replied to</param>
        /// <param name="reply">The text in the reply</param>
        public void ReplyToCheep(int cheepID, string reply, ChirpUser replyAuthorID);
        public void EditCheep(int cheepID, String newText);
    }
}
