using Chirp.Core.Models;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Services
{
    public class CheepService(ICheepRepository cheepRepo, UserManager<ChirpUser> userManager) : ICheepService
    {
        public List<CheepDTO> GetMainPageCheeps(int page = 0)
        {
            List<Cheep> cheeps = cheepRepo.GetMainPage(page).GetAwaiter().GetResult().ToList();
            return ToDTO(cheeps);
        }

        public List<CheepDTO> GetCheepsFromAuthorName(string authorName, int pagenum = 1)
        {
            ChirpUser? author = userManager.FindByNameAsync(authorName).GetAwaiter().GetResult();
            if (author == null) return [];
            return ToDTO(cheepRepo.GetAuthorPage(author, pagenum).GetAwaiter().GetResult());
        }

        public List<CheepDTO> GetCheepsFromAuthorID(string authorID, int pagenum = 1)
        {
            ChirpUser? author = userManager.FindByIdAsync(authorID.ToString()).GetAwaiter().GetResult();
            if (author == null) return [];

            return ToDTO(cheepRepo.GetAuthorPage(author, pagenum).GetAwaiter().GetResult());

        }

        public List<CheepDTO> GetCheepsFromAuthorEmail(string authorEmail, int pagenum = 1)
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
                string username = cheep.Author?.UserName ?? "Deleted User";
                string displayName = username.StartsWith("[")
                    ? "Deleted User"
                    : username;

                DTOCheeps.Add(
                    new CheepDTO(
                        cheep.Text,
                        cheep.TimeStamp.ToString(),
                        displayName,
                        cheep.CheepId,
                        cheep.ParentCheep?.CheepId,
                        ToDTO(cheep.Replies),
                        cheep.UsersWhoLiked.Count.ToString()
                    )
                );
            }
            return DTOCheeps;
        }

        public List<CheepDTO> GetOwnPrivateTimeline(string username, int pagenum = 0)
        {
            ChirpUser? user = userManager.FindByNameAsync(username).GetAwaiter().GetResult();

            if (user == null) return [];

            IEnumerable<Cheep> cheeps = cheepRepo.GetPrivateTimelineCheeps(user, pagenum).GetAwaiter().GetResult();
            return ToDTO(cheeps);
        }

        public string GetAuthorIDFromName(string authorName)
        {
            ChirpUser? author = userManager.FindByNameAsync(authorName).GetAwaiter().GetResult();
            if (author == null) throw new Exception($"No such author exists with name {authorName}");
            return author.Id;
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
                TimeStamp = DateTime.Now,
                UsersWhoLiked = new List<ChirpUser>()
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

        public List<CheepDTO> GetAllCheepsFromAuthorName(string authorName)
        {
            ChirpUser? author = userManager.FindByNameAsync(authorName).GetAwaiter().GetResult();
            if (author == null) throw new Exception($"No such author exists with name {authorName}");
            return ToDTO(cheepRepo.GetAllAuthorCheeps(author).GetAwaiter().GetResult());
        }

        public void LikeCheep(int cheepId, string userId)
        {
            cheepRepo.LikeCheep(cheepId, userId).GetAwaiter().GetResult();
        }

        public void UnLikeCheep(int cheepId, string userId)
        {
            cheepRepo.UnLikeCheep(cheepId, userId).GetAwaiter().GetResult();
        }
    }
}
