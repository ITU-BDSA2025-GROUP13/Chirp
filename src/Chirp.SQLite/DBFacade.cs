using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Models;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.SQLite
{
    public class DBFacade : IChirpFacade
    {
        private readonly string _sqlDBFilePath;
        private readonly int _readLimit = 32;

        public DBFacade()
        {
            string defaultDBPath = Path.Combine(Path.GetTempPath(), "chirp.db");
            // Use CHIRPDBPATH from env if present
            string? dbPathFromEnv = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            _sqlDBFilePath = string.IsNullOrEmpty(dbPathFromEnv) ? defaultDBPath : dbPathFromEnv;

            if (!File.Exists(_sqlDBFilePath))
            {
                string? dbDir = Path.GetDirectoryName(_sqlDBFilePath);
                // Must check if dbDir is null or empty, else compiler warns
                if (!string.IsNullOrEmpty(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }
                File.Create(_sqlDBFilePath).Close();
            }
            using (SqliteConnection connection = new SqliteConnection($"Data Source={_sqlDBFilePath}"))
            {
                string initilizationQuery = @"
                CREATE TABLE IF NOT EXISTS author (
                  author_id INTEGER PRIMARY KEY AUTOINCREMENT,
                  username STRING NOT NULL,
                  email STRING NOT NULL
                );

                CREATE TABLE IF NOT EXISTS cheep (
                  message_id INTEGER PRIMARY KEY AUTOINCREMENT,
                  author_id INTEGER REFERENCES author(author_id),
                  text STRING NOT NULL,
                  pub_date DATETIME NOT NULL
                );";

                connection.Open();
                using (SqliteCommand command = new SqliteCommand(initilizationQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertAuthor(Author author)
        {
            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO author(username, email)
                VALUES (@username, @email)";
            command.Parameters.AddWithValue("@username", author.Name);
            command.Parameters.AddWithValue("@email", author.Email);
            command.ExecuteNonQuery();
        }

        public void Create(Cheep cheep)
        {
            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO cheep (author_id, text, pub_date)
                VALUES (@AuthorID, @Text, @PubDate)";

            command.Parameters.AddWithValue("@AuthorID", cheep.Author.AuthorID);
            command.Parameters.AddWithValue("@Text", cheep.Text);
            command.Parameters.AddWithValue("@PubDate", cheep.TimeStamp);

            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new Exception("Insert failed — no rows affected.");
            }
        }

        public IEnumerable<Cheep> ReadPage(int pagenum = 0)
        {
            var results = new List<Cheep>();

            var queryString = @"
                SELECT c.text, c.pub_date, a.author_id, a.username, a.email
                FROM cheep c JOIN author a 
                ON c.author_id = a.author_id
                ORDER BY c.pub_date DESC
                LIMIT @limit OFFSET @offset";

            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@limit", _readLimit);
            command.Parameters.AddWithValue("@offset", pagenum * _readLimit);

            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
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

        public Author? ReadPageFromAuthor(string username, int pagenum = 0)
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

            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@limit", _readLimit);
            command.Parameters.AddWithValue("@offset", pagenum * _readLimit);

            connection.Open();

            using var reader = command.ExecuteReader();
            List<Cheep> cheeps = new List<Cheep>();
            while (reader.Read())
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

            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
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
    }
}
