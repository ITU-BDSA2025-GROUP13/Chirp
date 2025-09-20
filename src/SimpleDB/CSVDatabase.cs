using System.Globalization;
using CsvHelper;
using System.IO;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static string filePath = "../../assets/chirp_cli_db.csv";

    private static CSVDatabase<T>? _instance;

    private CSVDatabase() { }

    public static CSVDatabase<T> GetInstance()
    {
        if (_instance == null)
        {
            // Make sure the directory exists
            string? dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir); // creates folder if needed
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
                using var writer = new StreamWriter(filePath);
                writer.WriteLine("Author,Message,Timestamp");
            }
            _instance = new CSVDatabase<T>();
        }
        return _instance;
    }

    internal static void SetPathForTest(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty.", nameof(path));

        filePath = path;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        // Not using "using" keyword, as an IEnum is returned. Can be changed at some point.
        var csvReader = new CsvReader(new StreamReader(filePath), CultureInfo.InvariantCulture);

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
        using var csvWriter = new CsvWriter(new StreamWriter(filePath, append: true), CultureInfo.InvariantCulture);
        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
    }
}
