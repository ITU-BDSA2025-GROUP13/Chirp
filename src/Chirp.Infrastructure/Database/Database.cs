using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.Infrastructure
{
    public class Database
    {
        public readonly string? sqlDBFilePath;
        public readonly int readLimit = 32;

        public Database()
        {
            string defaultDBPath = Path.Combine(Path.GetTempPath(), "chirp.db");
            // Use CHIRPDBPATH from env if present
            string? dbPathFromEnv = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            sqlDBFilePath = string.IsNullOrEmpty(dbPathFromEnv) ? defaultDBPath : dbPathFromEnv;

            if (!File.Exists(sqlDBFilePath))
            {
                string? dbDir = Path.GetDirectoryName(sqlDBFilePath);
                // Must check if dbDir is null or empty, else compiler warns
                if (!string.IsNullOrEmpty(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                File.Create(sqlDBFilePath);
            }

            InitializeSchema(sqlDBFilePath);
        }

        private void InitializeSchema(string sqlDBFilePath)
        {

            using var connection = new SqliteConnection($"Data Source={sqlDBFilePath}");
            connection.Open();
            string initilizationQuery = @"
                CREATE TABLE IF NOT EXISTS author (
                  author_id INTEGER PRIMARY KEY AUTOINCREMENT,
                  username STRING NOT NULL,
                  email STRING NOT NULL
                );

                CREATE TABLE IF NOT EXISTS cheep (
                  message_id INTEGER PRIMARY KEY AUTOINCREMENT,
                  author_id INTEGER NOT NULL REFERENCES author(author_id),
                  text STRING NOT NULL,
                  pub_date DATETIME NOT NULL
                );";

            var command = new SqliteCommand(initilizationQuery, connection);
            command.ExecuteReader();
        }

    }
}
