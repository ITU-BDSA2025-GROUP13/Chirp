using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class AuthorRepository(IChirpDbContext dbContext) : IAuthorRepository
{
    #region INSERT
    public void InsertAuthor(Author author)
    {
        dbContext.Authors.Add(author);
        dbContext.SaveChanges();
    }
    #endregion
    
    #region UPDATE
    
    #endregion
    
    #region GET
    public Author? GetAuthorByName(string name)
    {
        return dbContext.Authors
            .Include(m => m.Cheeps)
            .FirstOrDefault(u => u.Name == name);
    }
    public Author? GetAuthorByEmail(string email)
    {
        return dbContext.Authors
            .Include(m => m.Cheeps)
            .FirstOrDefault(u => u.Email == email);
    }
    public Author? GetAuthorByID(int authorID)
    {
        return dbContext.Authors
            .Include(m => m.Cheeps)
            .FirstOrDefault(u => u.AuthorId == authorID);
    }

    public List<Author> GetAllAuthors()
    {
        return dbContext.Authors.ToList();
    }
    #endregion
}