using Chirp.Domain;

namespace Chirp.Infrastructure
{
    public class CheepController
    {
        private readonly ICheepRepository _repository;

        public CheepController()
        {
            var database = Database.CreateAsync().GetAwaiter().GetResult();
            _repository = CheepRepository.CreateAsync(database).GetAwaiter().GetResult();
        }

        public List<Cheep> GetCheeps(int page = 0)
        {

            return _repository.ReadPageAsync(page).GetAwaiter().GetResult().ToList();
        }

        public List<Cheep> GetCheepsFromAuthor(string author, int page = 0)
        {
            return _repository.ReadPageFromAuthorAsync(author, page).GetAwaiter().GetResult().ToList();

        }

        public void PostCheep(Cheep cheep)
        {
            _repository.PostAsync(cheep).GetAwaiter().GetResult();
        }
    }
}
