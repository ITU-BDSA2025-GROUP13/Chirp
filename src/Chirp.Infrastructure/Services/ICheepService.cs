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
        /// As an authenticated user, get both my own Cheeps and the Cheeps of people I follow.
        /// </summary>
        /// <param name="username">The user to get the private timeline for</param>
        /// <param name="pagenum">Page index to get</param>
        /// <returns>A list of CheepDTOs related to own and followed users</returns>
        public List<CheepDTO> GetOwnPrivateTimeline(string username, int pagenum = 0);

        /// <summary>
        /// Returns a list of ChirpUsers, which the given user follows.
        /// </summary>
        /// <param name="username">Given user to check following relations</param>
        /// <returns>List of ChirpUsers</returns>
        public Task<List<ChirpUser>> GetListOfFollowers(string username);
        /// <summary>
        /// Returns a list of usernames, which the given user follows.
        /// </summary>
        /// <param name="username">Given user to check following relations</param>
        /// <returns>List of usernames</returns>
        public List<string> GetListOfFollowerNames(string username);

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
