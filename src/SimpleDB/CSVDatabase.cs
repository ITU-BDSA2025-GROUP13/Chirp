using System.Globalization;
using CsvHelper;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string filepath = "../../assets/chirp_cli_db.csv";
    private object fileLock = new object();

    private static CSVDatabase<T>? _instance;

    private CSVDatabase() { }

    public static CSVDatabase<T> GetInstance()
    {
        if (_instance == null)
        {
            _instance = new CSVDatabase<T>();
        }
        return _instance;
    }

    internal void SetPathForTest(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty.", nameof(path));
        lock (fileLock)
        {
            filepath = path;
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        lock (fileLock)
        {
            // Not using "using" keyword, as an IEnum is returned. Can be changed at some point.
            var csvReader = new CsvReader(new StreamReader(new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)), CultureInfo.InvariantCulture);

            var result = new List<T>();
            if (limit == null)
            {
                //Forces to list to avoid lazy read, and disposes before returning
                result = csvReader.GetRecords<T>().ToList();
                csvReader.Dispose();
                return result;
            }

            for (int i = 0; i < limit && csvReader.Read(); i++)
            {
                result.Add(csvReader.GetRecord<T>());
            }

            csvReader.Dispose(); //Disposes everything before unlocking to ensure no IOException on multiple read/writes

            return result;
        }
    }

    public void Store(T record)
    {
        lock (fileLock)
        {
            using var csvWriter = new CsvWriter(new StreamWriter(new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.None)), CultureInfo.InvariantCulture);
            csvWriter.WriteRecord(record);
            csvWriter.NextRecord();

            csvWriter.Flush(); //Flushes everything before unlocking to ensure no IOException on multiple read/writes
        }
    }
}
