using System.Data;
using Microsoft.Data.Sqlite;
using Chirp.Models;
using Chirp.DataBase;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.Repository
{
    public class CheepRepository : ICheepRepository
    {

        private DB _db;
        public CheepRepository(DB db)
        {
            _db = db;
        }

        public void Create(Cheep cheep)
        {
            using var connection = new SqliteConnection($"Data Source={_db.sqlDBFilePath}");
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

        public IEnumerable<Cheep> ReadPage(int pagenum = 0)
        {
            var results = new List<Cheep>();

            var queryString =
                @"SELECT m.*, u.username
                  FROM message m JOIN user u ON m.author_id = u.user_id
                  ORDER BY m.pub_date DESC
                  LIMIT @limit OFFSET @offset";

            using var connection = new SqliteConnection($"Data Source={_db.sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@limit", _db.readLimit);
            command.Parameters.AddWithValue("@offset", pagenum * _db.readLimit);

            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int unixTimeStamp = reader.GetInt32(reader.GetOrdinal("pub_date"));
                string dateTimeString = UnixTimeStampToDateTimeString(unixTimeStamp);
                var cheep = new Cheep(
                    reader.GetString(reader.GetOrdinal("username")),
                    reader.GetString(reader.GetOrdinal("text")),
                    dateTimeString
                );
                results.Add(cheep);
            }
            return results;
        }

        public IEnumerable<Cheep> ReadPageFromAuthor(string username, int pagenum = 0)
        {
            var results = new List<Cheep>();

            var queryString =
                @"SELECT m.*, u.username
                  FROM message m JOIN user u on m.author_id = u.user_id
                  WHERE u.username = @username
                  ORDER BY m.pub_date DESC
                  LIMIT @limit OFFSET @offset";

            using var connection = new SqliteConnection($"Data Source={_db.sqlDBFilePath}");
            using var command = connection.CreateCommand();
            command.CommandText = queryString;
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@limit", _db.readLimit);
            command.Parameters.AddWithValue("@offset", pagenum * _db.readLimit);

            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int unixTimeStamp = reader.GetInt32(reader.GetOrdinal("pub_date"));
                string dateTimeString = UnixTimeStampToDateTimeString(unixTimeStamp);
                var cheep = new Cheep(
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
