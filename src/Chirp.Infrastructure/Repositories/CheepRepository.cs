using Microsoft.EntityFrameworkCore;
using Chirp.Domain;

namespace Chirp.Infrastructure
{
    /// <summary>
    /// Repository for managing Cheep entities and their associated authors in the database.
    /// Implements the ICheepRepository interface to provide data access operations.
    /// </summary>
    /// <param name="dbContext">The database context used for data access operations.</param>
    public class CheepRepository : ICheepRepository
    {
        private readonly IChirpDbContext _dbContext;
        private readonly int _readLimit = 32;

        public CheepRepository(IChirpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Inserts a new author into the database as a User entity.
        /// </summary>
        /// <param name="author">The author to insert.</param>
        public void InsertAuthor(Author author)
        {
            var user = new Author
            {
                AuthorId = author.AuthorId,
                Name = author.Name,
                Email = author.Email,
                PasswordHash = ""
            };
            _dbContext.Authors.Add(user);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Posts a new cheep (message) to the database asynchronously.
        /// </summary>
        /// <param name="cheep">The cheep to post.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the author is not found in the database.</exception>
        public async Task PostAsync(Cheep cheep)
        {
            if (cheep.Author == null)
            {
                throw new ArgumentNullException(nameof(cheep.Author), "Cheep.Author cannot be null.");
            }

            var author = await _dbContext.Authors.FirstOrDefaultAsync(u => u.AuthorId == cheep.Author.AuthorId);
            if (author == null)
            {
                throw new Exception($"Author with ID '{cheep.Author.AuthorId}' not found.");
            }

            var message = new Cheep
            {
                CheepId = 0, // Assuming 0 for new entity, will be set by DB
                AuthorId = author.AuthorId,
                Author = author,
                Text = cheep.Text,
                TimeStamp = cheep.TimeStamp
            };

            _dbContext.Cheeps.Add(message);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of cheeps ordered by publication date (newest first).
        /// </summary>
        /// <param name="pagenum">The zero-based page number to retrieve. Default is 0.</param>
        /// <returns>A task containing an enumerable collection of cheeps for the specified page.</returns>
        public async Task<IEnumerable<Cheep>> ReadPageAsync(int pagenum = 0)
        {
            var messages = await _dbContext.Cheeps
                .Include(m => m.Author)
                .OrderByDescending(m => m.TimeStamp)
                .Skip(pagenum * _readLimit)
                .Take(_readLimit)
                .ToListAsync();

            var cheeps = messages.Select(m => new Cheep
            {
                CheepId = m.CheepId,
                AuthorId = m.AuthorId,
                Author = new Author
                {
                    AuthorId = m.Author!.AuthorId,
                    Name = m.Author.Name,
                    Email = m.Author.Email,
                    Cheeps = new List<Cheep>()
                },
                Text = m.Text,
                TimeStamp = m.TimeStamp
            });
            return cheeps;
        }

        /// <summary>
        /// Retrieves an author with a paginated list of their cheeps ordered by publication date (newest first).
        /// </summary>
        /// <param name="username">The username of the author to retrieve.</param>
        /// <param name="pagenum">The zero-based page number to retrieve. Default is 0.</param>
        /// <returns>A task containing the author with their cheeps, or null if the author is not found.</returns>
        public async Task<Author?> ReadPageFromAuthorAsync(string username, int pagenum = 0)
        {
            var user = await _dbContext.Authors.FirstOrDefaultAsync(u => u.Name == username);
            if (user == null)
                return null;

            var messages = await _dbContext.Cheeps
                .Where(m => m.AuthorId == user.AuthorId)
                .OrderByDescending(m => m.TimeStamp)
                .Skip(pagenum * _readLimit)
                .Take(_readLimit)
                .ToListAsync();

            var cheeps = messages.Select(m => new Cheep
            {
                CheepId = m.CheepId,
                AuthorId = m.AuthorId,
                Author = new Author()
                {
                    AuthorId = user.AuthorId,
                    Name = user.Name,
                    Email = user.Email,
                    Cheeps = new List<Cheep>()
                },
                Text = m.Text,
                TimeStamp = m.TimeStamp
            }).ToList();

            return new Author
            {
                AuthorId = user.AuthorId,
                Name = user.Name,
                Email = user.Email,
                Cheeps = cheeps
            };
        }

        /// <summary>
        /// Retrieves an author by their username.
        /// </summary>
        /// <param name="username">The username of the author to retrieve.</param>
        /// <returns>The author if found, otherwise null.</returns>
        public Author? GetAuthorFromUsername(string username)
        {
            var user = _dbContext.Authors.FirstOrDefault(u => u.Name == username);
            if (user == null)
                return null;

            return new Author
            {
                AuthorId = user.AuthorId,
                Name = user.Name,
                Email = user.Email,
                Cheeps = new List<Cheep>()
            };
        }

        /// <summary>
        /// Retrieves an author by their author ID.
        /// </summary>
        /// <param name="authorId">The ID of the author to retrieve.</param>
        /// <returns>The author if found, otherwise null.</returns>
        public Author? GetAuthorFromAuthorID(int authorId)
        {
            var user = _dbContext.Authors.FirstOrDefault(u => u.AuthorId == authorId);
            if (user == null)
                return null;

            return new Author
            {
                AuthorId = user.AuthorId,
                Name = user.Name,
                Email = user.Email,
                Cheeps = new List<Cheep>()
            };
        }
    }
}
