using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Models;

namespace Chirp.SQLite
{
    public class DBFacade : IChirpFacade
    {
        private readonly string _sqlDBFilePath;

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
                );";

                connection.Open();
                using (SqliteCommand command = new SqliteCommand(initilizationQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Create(CheepViewModel cheep)
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

        public IEnumerable<CheepViewModel> Read(string? username = null, int n = 0)
        {
            var results = new List<CheepViewModel>();
            var queryString = @"
                    SELECT m.*, u.username FROM message m
                    JOIN user u ON m.author_id = u.user_id
                    WHERE (@name IS NULL OR u.username = @name)
                    ORDER BY m.pub_date DESC
                    LIMIT @limit";

            using var connection = new SqliteConnection($"Data Source={_sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@name", (object?)username ?? DBNull.Value);
            command.Parameters.AddWithValue("@limit", n == 0 ? -1 : n);

            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int unixTimeStamp = reader.GetInt32(reader.GetOrdinal("pub_date"));
                string dateTimeString = UnixTimeStampToDateTimeString(unixTimeStamp);
                var cheep = new CheepViewModel(
                    reader.GetString(reader.GetOrdinal("username")),
                    reader.GetString(reader.GetOrdinal("text")),
                    dateTimeString
                );
                results.Add(cheep);
            }
            return results;
        }

        private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}
