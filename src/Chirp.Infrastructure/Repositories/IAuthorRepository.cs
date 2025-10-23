using Chirp.Core.Models;

namespace Chirp.Infrastructure.Repositories;

public interface IAuthorRepository
{
    #region INSERT
    /// <summary>
    /// Inserts a new author into the database as a User entity.
    /// </summary>
    /// <param name="author">The author to insert.</param>
    void InsertAuthor(Author author);
    #endregion

    #region UPDATE
    #endregion

    #region GET
    /// <summary>
    /// Retrieves an author by their username.
    /// </summary>
    /// <param name="name">The username of the author to retrieve.</param>
    /// <returns>The author if found, otherwise null.</returns>
    Author? GetAuthorByName(string name);
    /// <summary>
    /// Retrieves an author by their email.
    /// </summary>
    /// <param name="email">The email of the author to retrieve.</param>
    /// <returns>The author if found, otherwise null.</returns>
    Author? GetAuthorByEmail(string email);
    /// <summary>
    /// Retrieves an author by their author ID.
    /// </summary>
    /// <param name="authorID">The ID of the author to retrieve.</param>
    /// <returns>The author if found, otherwise null.</returns>
    Author? GetAuthorByID(int authorID);
    /// <returns>All authors in the DB</returns>
    List<Author> GetAllAuthors();
    #endregion
}