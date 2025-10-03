using Microsoft.Data.Sqlite;
using Chirp.Domain;

namespace Chirp.Infrastructure
{
    public class CheepRepository : ICheepRepository
    {

        private Database _database;

        public CheepRepository(Database database)
        {
            _database = database;
        }

        public void InsertAuthor(Author author)
        {
            using var connection = new SqliteConnection($"Data Source={_database.sqlDBFilePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO author(username, email)
                VALUES (@username, @email)";
            command.Parameters.AddWithValue("@username", author.Name);
            command.Parameters.AddWithValue("@email", author.Email);
            command.ExecuteNonQuery();
        }

        public async Task PostAsync(Cheep cheep)
        {
            using var connection = new SqliteConnection($"Data Source={_database.sqlDBFilePath}");
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO cheep (author_id, text, pub_date)
                VALUES (@AuthorId, @Text, @PubDate)";

            command.Parameters.AddWithValue("@AuthorId", cheep.Author.AuthorID);
            command.Parameters.AddWithValue("@Text", cheep.Text);
            command.Parameters.AddWithValue("@PubDate", cheep.TimeStamp);

            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                throw new Exception("Insert failed — no rows affected.");
            }
        }

        public async Task<IEnumerable<Cheep>> ReadPageAsync(int pagenum = 0)
        {

            var results = new List<Cheep>();
            var queryString = @"
                SELECT c.text, c.pub_date, a.author_id, a.username, a.email
                FROM cheep c JOIN author a 
                ON c.author_id = a.author_id
                ORDER BY c.pub_date DESC
                LIMIT @limit OFFSET @offset";

            using var connection = new SqliteConnection($"Data Source={_database.sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@limit", _database.readLimit);
            command.Parameters.AddWithValue("@offset", pagenum * _database.readLimit);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var author = new Author(
                     reader.GetInt32(reader.GetOrdinal("author_id")),
                     reader.GetString(reader.GetOrdinal("username")),
                     reader.GetString(reader.GetOrdinal("email")),
                     new List<Cheep>()
                );

                var cheep = new Cheep(
                    reader.GetString(reader.GetOrdinal("text")),
                    reader.GetDateTime(reader.GetOrdinal("pub_date")),
                    author
                );

                results.Add(cheep);
            }
            return results;
        }

        public async Task<Author?> ReadPageFromAuthorAsync(string username, int pagenum = 0)
        {
            var author = GetAuthorFromUsername(username);
            if (author == null) return null;

            var queryString = @"
                SELECT c.text, c.pub_date
                FROM cheep c join author a 
                ON c.author_id = a.author_id
                WHERE a.username = @username
                ORDER BY c.pub_date DESC
                LIMIT @limit OFFSET @offset";

            using var connection = new SqliteConnection($"Data Source={_database.sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@limit", _database.readLimit);
            command.Parameters.AddWithValue("@offset", pagenum * _database.readLimit);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            List<Cheep> cheeps = new List<Cheep>();
            while (await reader.ReadAsync())
            {
                var cheep = new Cheep(
                    reader.GetString(reader.GetOrdinal("text")),
                    reader.GetDateTime(reader.GetOrdinal("pub_date")),
                    author
                );
                cheeps.Add(cheep);
            }
            return author with { Cheeps = cheeps };
        }

        public Author? GetAuthorFromUsername(string username)
        {
            var queryString =
                @"SELECT author_id, username, email
                  FROM author
                  WHERE username = @username
                  LIMIT 1;";

            using var connection = new SqliteConnection($"Data Source={_database.sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@username", username);
            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Author(
                    reader.GetInt32(reader.GetOrdinal("author_id")),
                    reader.GetString(reader.GetOrdinal("username")),
                    reader.GetString(reader.GetOrdinal("email")),
                    new List<Cheep>()
                );
            }
            return null;
        }

        private static string UnixTimestampToDateTimeString(double unixTimeStamp)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}
