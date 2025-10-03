using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Domain;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.Infrastructure
{
    public class CheepRepository : ICheepRepository
    {

        private Database _database;

        public CheepRepository(Database database)
        {
            _database = database;
        }

        public async Task PostAsync(Cheep cheep)
        {
            using var connection = new SqliteConnection($"Data Source={_database.sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO message (author_id, text, pub_date)
                VALUES (@AuthorId, @Text, @PubDate)";

            command.Parameters.AddWithValue("@AuthorId", cheep.Author.GetHashCode());
            command.Parameters.AddWithValue("@Text", cheep.Message);
            command.Parameters.AddWithValue("@PubDate", cheep.Timestamp);

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
            var queryString =
                @"SELECT m.*, u.username
                  FROM message m JOIN user u ON m.author_id = u.user_id
                  ORDER BY m.pub_date DESC
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
                int unixTimeStamp = reader.GetInt32(reader.GetOrdinal("pub_date"));
                string dateTimeString = UnixTimestampToDateTimeString(unixTimeStamp);
                var cheep = new Cheep(
                    reader.GetString(reader.GetOrdinal("username")),
                    reader.GetString(reader.GetOrdinal("text")),
                    dateTimeString
                );
                results.Add(cheep);
            }
            return results;
        }

        public async Task<IEnumerable<Cheep>> ReadPageFromAuthorAsync(string username, int pagenum = 0)
        {
            var results = new List<Cheep>();

            var queryString =
                @"SELECT m.*, u.username
                  FROM message m JOIN user u on m.author_id = u.user_id
                  WHERE u.username = @username
                  ORDER BY m.pub_date DESC
                  LIMIT @limit OFFSET @offset";

            using var connection = new SqliteConnection($"Data Source={_database.sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@limit", _database.readLimit);
            command.Parameters.AddWithValue("@offset", pagenum * _database.readLimit);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int unixTimeStamp = reader.GetInt32(reader.GetOrdinal("pub_date"));
                string dateTimeString = UnixTimestampToDateTimeString(unixTimeStamp);
                var cheep = new Cheep(
                    reader.GetString(reader.GetOrdinal("username")),
                    reader.GetString(reader.GetOrdinal("text")),
                    dateTimeString
                );
                results.Add(cheep);
            }
            return results;
        }

        private static string UnixTimestampToDateTimeString(double unixTimeStamp)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}
