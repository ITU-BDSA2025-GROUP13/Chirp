using Chirp.Models;
using Chirp.SQLite;
using Microsoft.Data.Sqlite;

public class ChirpSQLiteTests
{
    [Fact]
    public void ReadAllMessageTest()
    {
        var db = CreatesDatabase();
        IEnumerable<CheepViewModel> cheeps = db.Read();

        foreach (CheepViewModel cheep in cheeps)
        {
            Assert.Equal("hello world", cheep.Message);
            Assert.Equal("Jacqualine Gilcoine", cheep.Author);
        }
    }

    // [Fact]
    // public void ReadSomeMessageTest()
    // {
    //      TODO
    // }

    // [Fact]
    // public void ReadAllUsersTest()
    // {
    //      TODO
    // }

    // [Fact]
    // public void ReadSomeUsersTest()
    // {
    //      TODO
    // }

    private IChirpFacade CreatesDatabase()
    {
        var connectionString = "Data Source=:memory:;Cache=Shared";
        var db = new DBFacade(connectionString);

        using var connection = new SqliteConnection($"Data Source={connectionString}");
        using var command = connection.CreateCommand();
        command.CommandText = @"

                drop table if exists user;
                create table user (
                  user_id integer primary key autoincrement,
                  username string not null,
                  email string not null,
                  pw_hash string not null
                );

                drop table if exists message;
                create table message (
                  message_id integer primary key autoincrement,
                  author_id integer not null,
                  text string not null,
                  pub_date integer
                );

                INSERT INTO user (user_id, username, email, pw_hash)
                VALUES (10, 'Jacqualine Gilcoine', 'Jacqualine.Gilcoine@gmail.com', 'password');

                INSERT INTO message (message_id, author_id, text, pub_date)
                VALUES (0, 10, 'hello world', 1690895677);
            ";

        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();

        return db;
    }
}
