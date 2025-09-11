using System.Globalization;
using CsvHelper;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private string filepath = "../../assets/chirp_cli_db.csv";

    private CSVDatabase() { }

    private static CSVDatabase<T> _instance;

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

        filepath = path;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        // Not using "using" keyword, as an IEnum is returned. Can be changed at some point.
        var csvReader = new CsvReader(new StreamReader(filepath), CultureInfo.InvariantCulture);

        if (limit == null) return csvReader.GetRecords<T>();

        var result = new List<T>();
        for (int i = 0; i < limit && csvReader.Read(); i++)
        {
            result.Add(csvReader.GetRecord<T>());
        }
        return result;
    }

    public void Store(T record)
    {
        using var csvWriter = new CsvWriter(new StreamWriter(filepath, append: true), CultureInfo.InvariantCulture);
        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
    }
}
