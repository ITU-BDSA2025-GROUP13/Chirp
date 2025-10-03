using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Models;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.DataBase
{
    public class DB
    {
        public readonly string sqlDBFilePath;
        public readonly int readLimit = 32;

        public DB()
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
            using (SqliteConnection connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
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
    }
}
