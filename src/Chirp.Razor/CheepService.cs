using Chirp.Domain;
using Chirp.Infrastructure;

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
        public List<CheepDTO> GetCheepsFromAuthor(string username, int pagenum);
    }

    public class CheepService : ICheepService
    {

        private readonly ICheepRepository _repository;

        public CheepService()
        {
            Database database = new Database();
            _repository = new CheepRepository(database);
        }

        public List<CheepDTO> GetCheeps(int page = 0)
        {
            List<Cheep> cheeps = _repository.ReadPageAsync(page).GetAwaiter().GetResult().ToList();
            List<CheepDTO> DTOCheeps = new List<CheepDTO>();
            foreach (Cheep cheep in cheeps)
            {
                DTOCheeps.Add(
                    new CheepDTO(
                        cheep.Text,
                        cheep.TimeStamp.ToString(),
                        cheep.Author.Name
                    )
                );
            }
            return DTOCheeps;
        }

        public List<CheepDTO> GetCheepsFromAuthor(string author, int page = 0)
        {
            // Return empty list if no such author
            var result = _repository.ReadPageFromAuthorAsync(author, page).GetAwaiter().GetResult();
            if (result == null) return new List<CheepDTO>();

            List<Cheep> cheeps = result.Cheeps.ToList();
            List<CheepDTO> DTOCheeps = new List<CheepDTO>();
            foreach (Cheep cheep in cheeps)
            {
                DTOCheeps.Add(
                    new CheepDTO(
                        cheep.Text,
                        cheep.TimeStamp.ToString(),
                        cheep.Author.Name
                    )
                );
            }
            return DTOCheeps;
        }


        public void PostCheep(String text, int authorID)
        {
            Author? author = _repository.GetAuthorFromAuthorID(authorID);
            if (author == null)
            {
                throw new Exception($"No such author exists {authorID}");
            }

            Cheep cheep = new Cheep(
                text,
                DateTime.Now,
                author
            );
            _repository.PostAsync(cheep).GetAwaiter().GetResult();
        }
    }
}
