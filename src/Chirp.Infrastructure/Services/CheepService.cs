using Chirp.Domain;
using Chirp.Infrastructure;

namespace Chirp.Razor
{
    public class CheepService(ICheepRepository cheepRepository) : ICheepService
    {
        private readonly ICheepRepository _repository = cheepRepository;

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
                        cheep.Author?.Name ?? string.Empty
                    )
                );
            }
            return DTOCheeps;
        }

        public List<CheepDTO> GetCheepsFromAuthor(string author, int page = 0)
        {
            // Return empty list if no such author
            var result = _repository.ReadPageFromAuthorAsync(author, page).GetAwaiter().GetResult();
            if (result == null) return [];

            List<Cheep> cheeps = result.Cheeps.ToList();
            List<CheepDTO> DTOCheeps = new List<CheepDTO>();
            foreach (Cheep cheep in cheeps)
            {
                DTOCheeps.Add(
                    new CheepDTO(
                        cheep.Text,
                        cheep.TimeStamp.ToString(),
                        cheep.Author?.Name ?? string.Empty
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

            Cheep cheep = new Cheep
            {
                AuthorId = authorID,
                Author = author,
                Text = text,
                TimeStamp = DateTime.Now
            };
            _repository.PostAsync(cheep).GetAwaiter().GetResult();
        }
    }
}
