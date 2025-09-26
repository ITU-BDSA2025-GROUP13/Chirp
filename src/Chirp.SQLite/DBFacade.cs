using System.Data;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using Chirp.Models;
using System.Net.WebSockets;
namespace Chirp.SQLite
{
    public class DBFacade : IChirpFacade
    {
        private static readonly string _sqlDBFilePath = "/tmp/chirp.db";

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

        private static void ReadSingleRow(IDataRecord dataRecord)
        {
            Console.WriteLine(String.Format("{0}, {1}", dataRecord[0], dataRecord[1]));
        }

        private static string? GetUserNameFromUserID(int userId)
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
