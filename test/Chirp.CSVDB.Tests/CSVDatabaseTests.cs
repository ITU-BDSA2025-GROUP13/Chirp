using SimpleDB;
using Chirp.CLI.Models;

namespace Chirp.CSVDB.Tests;

public class CSVDatabaseTests
{
    private readonly CSVDatabase<Cheep> database;
    private int sampleSize = 4; //The amount of samples inside "testSample.csv"
    private readonly String sampleDB = "../../../assets/testSample.csv";

    public CSVDatabaseTests()
    {
        database = CSVDatabase<Cheep>.GetInstance();
        database.SetPathForTest(sampleDB);
    }

    [Fact]
    public void singletonDatabase_ReturnsSameObject()
    {
        CSVDatabase<Cheep> database2 = CSVDatabase<Cheep>.GetInstance();
        Assert.Same(database2, database);
    }

    #region Read
    [Fact]
    public void readWithoutLimit_ReturnsAll()
    {
        int expectedNumberOfChirps = sampleSize;
        int actual = database.Read().Count();
        Assert.Equal(expectedNumberOfChirps, actual);
    }

    [Fact]
    public void readWithLimit_Returns2()
    {
        int expectedNumberOfChirps = 2;
        int actual = database.Read(expectedNumberOfChirps).Count();
        Assert.Equal(expectedNumberOfChirps, actual);
    }

    [Fact]
    public void readOverLimit_ReturnsAll()
    {
        int expectedNumberOfChirps = sampleSize;
        int actual = database.Read(sampleSize * 2).Count();
        Assert.Equal(expectedNumberOfChirps, actual);
    }


    [Fact]
    public void readUnderZero_ReturnsEmptyList()
    {
        Assert.False(database.Read(-1).Any());
    }

    [Fact]
    public void readZero_ReturnsEmptyList()
    {
        Assert.False(database.Read(0).Any());
    }
    #endregion

    #region Write
    // NOTE: This write test depends on the read tests passing
    [Fact]
    public void store_writesToDisk()
    {
        // Override current db path
        string writeTestPath = "../../../assets/writeTest.csv";
        File.Copy(sampleDB, writeTestPath);
        database.SetPathForTest(writeTestPath);

        int origCount = database.Read().Count();
        int amount = 3;
        long unixTimestamp = 1609459200;
        for (int i = 0; i < amount; i++)
        {
            var placeholder = "Test" + i;
            database.Store(new Cheep(placeholder, placeholder, unixTimestamp));
        }
        // Check that we added 3 entries to db
        Assert.Equal(amount, database.Read().Count() - origCount);
        File.Delete(writeTestPath);

        // Reset db path
        database.SetPathForTest(sampleDB);
    }
    #endregion
}
