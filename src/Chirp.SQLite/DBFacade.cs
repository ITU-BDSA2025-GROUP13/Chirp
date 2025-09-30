using System.Data;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using Chirp.Models;
using System.Net.WebSockets;
namespace Chirp.SQLite
{
    public class DBFacade : IChirpFacade
    {
        private readonly string _sqlDBFilePath;

        public DBFacade()
        {
            // Use CHIRPDBPATH from env if present
            string? dbPathFromEnv = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            _sqlDBFilePath = string.IsNullOrEmpty(dbPathFromEnv) ? "/tmp/chirp.db" : dbPathFromEnv;

            if (!File.Exists(_sqlDBFilePath))
            {
                string? dbDir = Path.GetDirectoryName(_sqlDBFilePath);
                // Must check if dbDir is null or empty, else compiler warns
                if (!string.IsNullOrEmpty(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                File.Create(_sqlDBFilePath);
            }
            using (SqliteConnection connection = new SqliteConnection($"Data Source={_sqlDBFilePath}"))
            {
                string initilizationQuery = @"
                CREATE TABLE IF NOT EXISTS user (
                  user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                  username STRING NOT NULL,
                  email STRING NOT NULL,
                  pw_hash STRING NOT NULL
                );

                CREATE TABLE IF NOT EXISTS message (
                  message_id INTEGER PRIMARY KEY AUTOINCREMENT,
                  author_id INTEGER NOT NULL,
                  text STRING NOT NULL,
                  pub_date INTEGER
                )";

                connection.Open();
                using (SqliteCommand command = new SqliteCommand(initilizationQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Post(CheepViewModel cheep)
        {
            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO message (author_id, text, pub_date)
                VALUES (@AuthorId, @Text, @PubDate)";

            command.Parameters.AddWithValue("@AuthorId", cheep.Author.GetHashCode());
            command.Parameters.AddWithValue("@Text", cheep.Message);
            command.Parameters.AddWithValue("@PubDate", cheep.Timestamp);

            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                throw new Exception("Insert failed — no rows affected.");
            }
        }

        public IEnumerable<CheepViewModel> Read(int n = 0)
        {
            var results = new List<CheepViewModel>();
            var queryString = n == 0
                ? @"SELECT * FROM message ORDER BY pub_date DESC"
                : $"SELECT * FROM message ORDER BY pub_date DESC LIMIT {n}";

            Console.WriteLine(queryString);

            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var userId = reader.GetInt32(reader.GetOrdinal("author_id"));
                var userName = GetUserNameFromUserID(userId) ?? throw new Exception("User not found! user_id: " + userId);
                var cheep = new CheepViewModel(
                    userName,
                    reader.GetString(reader.GetOrdinal("text")),
                    reader.GetInt32(reader.GetOrdinal("pub_date")).ToString()
                );
                results.Add(cheep);
            }
            return results;
        }

        public IEnumerable<CheepViewModel> ReadFromAuthor(string user, int n = 0)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<CheepViewModel> ReadAll()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<CheepViewModel> ReadAllCheeps()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<CheepViewModel> ReadCheeps(int n)
        {
            throw new NotImplementedException();
        }

        private void ReadSingleRow(IDataRecord dataRecord)
        {
            Console.WriteLine(String.Format("{0}, {1}", dataRecord[0], dataRecord[1]));
        }

        private string? GetUserNameFromUserID(int userId)
        {
            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT username FROM user WHERE user_id = @id";
            command.Parameters.AddWithValue("@id", userId);

            connection.Open();
            var result = command.ExecuteScalar();
            return result?.ToString();
        }

    }
}
