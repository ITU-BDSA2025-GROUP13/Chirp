using SimpleDB;
using Chirp.CLI.Models;

namespace Chirp.CSVDB.Tests;

public class CSVDatabaseTests
{
    private readonly CSVDatabase<Cheep> database;
    private int sampleSize = 4; //The amount of samples inside "testSample.csv" 
    
    public CSVDatabaseTests()
    {
        database = new CSVDatabase<Cheep>("../../../assets/testSample.csv");
    }

    /*
    Waiting for Database to be a singleton before this test works
    [Fact]
    public void singletonDatabase_ReturnsSameObject()
    {
        CSVDatabase<Cheep> database2 = CSVDatabase<Cheep>("../../../assets/testSample.csv").Instance;
        Assert.Same(database2, database.Instance);
    }
    */

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
        string writeTestPath = "../../../assets/writeTest.csv";
        CSVDatabase<Cheep> writeDatabase = new CSVDatabase<Cheep>(writeTestPath);
        int amount = 3;
        long unixTimestamp = 1609459200;
        for (int i = 0; i < amount; i++)
        {
            var placeholder = "Test" + i;
            writeDatabase.Store(new Cheep(placeholder, placeholder, unixTimestamp));
        }
        Assert.Equal(amount, database.Read().Count()-1);
        File.Delete(writeTestPath);
    }
    #endregion
}