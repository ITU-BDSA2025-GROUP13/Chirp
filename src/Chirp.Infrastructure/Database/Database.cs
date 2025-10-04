using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Domain;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.Infrastructure
{
    public class Database
    {
        public readonly string sqlDBFilePath;
        public readonly int readLimit = 32;

        private Database(string sqlDBFilePath)
        {
            this.sqlDBFilePath = sqlDBFilePath;
        }

        public static async Task<Database> CreateAsync()
        {
            string defaultDBPath = Path.Combine(Path.GetTempPath(), "chirp.db");
            // Use CHIRPDBPATH from env if present
            string? dbPathFromEnv = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            string sqlDBFilePath = string.IsNullOrEmpty(dbPathFromEnv) ? defaultDBPath : dbPathFromEnv;

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

            await InitializeSchemaAsync(sqlDBFilePath);

            return new Database(sqlDBFilePath);

        }

        private static async Task InitializeSchemaAsync(string sqlDBFilePath)
        {

            using var connection = new SqliteConnection($"Data Source={sqlDBFilePath}");
            await connection.OpenAsync();
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

            var command = new SqliteCommand(initilizationQuery, connection);
            await command.ExecuteReaderAsync();
        }

    }
}
