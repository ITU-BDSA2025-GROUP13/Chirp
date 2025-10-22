using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;

namespace Chirp.Infrastructure.Services
{
    public class CheepService(ICheepRepository cheepRepo, IAuthorRepository authorRepo) : ICheepService
    {
        public List<CheepDTO> GetMainPageCheeps(int page = 0)
        {
            List<Cheep> cheeps = cheepRepo.GetMainPage(page).GetAwaiter().GetResult().ToList();
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
        public List<CheepDTO> GetCheepsFromAuthorName(string authorName, int pagenum = 0)
        {
            Author? author = authorRepo.GetAuthorByName(authorName);
            if (author == null) return [];

            return ToDTO(author.Cheeps);
        }
        public List<CheepDTO> GetCheepsFromAuthorID(int authorID, int pagenum = 0)
        {
            Author? author = authorRepo.GetAuthorByID(authorID);
            if (author == null) return [];
            
            return ToDTO(author.Cheeps);
        }
        public List<CheepDTO> GetCheepsFromAuthorEmail(string authorEmail, int pagenum = 0)
        {
            Author? author = authorRepo.GetAuthorByEmail(authorEmail);
            if (author == null) return [];
            
            return ToDTO(author.Cheeps);
        }

        private List<CheepDTO> ToDTO(IEnumerable<Cheep> cheeps)
        {
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
            Author? author = authorRepo.GetAuthorByID(authorID);
            if (author == null) throw new Exception($"No such author exists {authorID}");
            
            Cheep cheep = new Cheep
            {
                AuthorId = authorID,
                Author = author,
                Text = text,
                TimeStamp = DateTime.Now
            };
            cheepRepo.InsertCheep(cheep).GetAwaiter().GetResult();
        }
    }
}
