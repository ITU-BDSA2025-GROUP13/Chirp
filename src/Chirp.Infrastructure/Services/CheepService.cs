using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Services
{
    public class CheepService(ICheepRepository cheepRepo, UserManager<ChirpUser> userManager) : ICheepService
    {
        private const string DeletedUser = "Deleted User";
        public List<CheepDTO> GetMainPageCheeps(int page = 0)
        {
            List<Cheep> cheeps = cheepRepo.GetMainPage(page).GetAwaiter().GetResult().ToList();
            return ToDTO(cheeps);
        }
        public List<CheepDTO> GetCheepsFromAuthorName(string authorName, int pagenum = 0)
        {
            ChirpUser? author = userManager.FindByNameAsync(authorName).GetAwaiter().GetResult();
            if (author == null) return [];
            return ToDTO(cheepRepo.GetAuthorPage(author, pagenum).GetAwaiter().GetResult());
        }
        public List<CheepDTO> GetCheepsFromAuthorID(string authorID, int pagenum = 0)
        {
            ChirpUser? author = userManager.FindByIdAsync(authorID.ToString()).GetAwaiter().GetResult();
            if (author == null) return [];

            return ToDTO(cheepRepo.GetAuthorPage(author, pagenum).GetAwaiter().GetResult());

        }
        public List<CheepDTO> GetCheepsFromAuthorEmail(string authorEmail, int pagenum = 0)
        {
            ChirpUser? author = userManager.FindByEmailAsync(authorEmail).GetAwaiter().GetResult();
            if (author == null) return [];

            return ToDTO(cheepRepo.GetAuthorPage(author, pagenum).GetAwaiter().GetResult());

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
                        cheep.Author.UserName ?? DeletedUser,
                        cheep.CheepId,
                        cheep.ParentCheep?.CheepId,
                        ToDTO(cheep.Replies)
                    )
                );
            }
            return DTOCheeps;
        }

        public void PostCheep(string text, string authorID)
        {
            ChirpUser? author = userManager.FindByIdAsync(authorID).GetAwaiter().GetResult();
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

        public void DeleteCheep(int cheepID)
        {
            Cheep? cheep = cheepRepo.GetCheepById(cheepID).GetAwaiter().GetResult();
            if (cheep == null) throw new Exception($"No such cheep exists {cheepID}");
            cheepRepo.DeleteCheep(cheep).GetAwaiter().GetResult();
        }

        public void ReplyToCheep(int cheepID, string reply, ChirpUser author)
        {
            Cheep? parentCheep = cheepRepo.GetCheepById(cheepID).GetAwaiter().GetResult();
            if (parentCheep == null) throw new ArgumentNullException($"No parentCheep cheep exists with ID: {cheepID}");

            Cheep replyCheep = new Cheep
            {
                AuthorId = author.Id,
                Author = author,
                Text = reply,
                TimeStamp = DateTime.Now,
                ParentCheep = parentCheep
            };
            cheepRepo.InsertCheep(replyCheep).GetAwaiter().GetResult();
        }

        public void EditCheep(int cheepId, string newText)
        {
            cheepRepo.EditCheepById(cheepId, newText).GetAwaiter().GetResult();
        }
    }
}
