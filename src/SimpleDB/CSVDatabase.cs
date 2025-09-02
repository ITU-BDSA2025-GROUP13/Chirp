using System.Collections;
using System.Globalization;
using CsvHelper;
using MissingFieldException = System.MissingFieldException;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T> {
    private readonly string filepath = "../../assets/chirp_cli_db.csv";
    
    public IEnumerable<T> Read(int? limit = null) {
        // Not using "using" keyword, as an IEnum is returned. Can be changed at some point.
        var csvReader = new CsvReader(new StreamReader(filepath), CultureInfo.InvariantCulture);
        
        if (limit == null) return csvReader.GetRecords<T>();
        
        var result = new List<T>();
        for (int i = 0; i < limit; i++) {
            try {
                csvReader.Read();
                result.Add(csvReader.GetRecord<T>());
            }
            catch (CsvHelperException) { //End of file
                break;
            }
        }
        return result;
    }

    public void Store(T record) {
        using var csvWriter = new CsvWriter(new StreamWriter(filepath, append: true), CultureInfo.InvariantCulture);
        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
    }
}