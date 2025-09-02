namespace Chirp.CLI;
using Models;
using System.Globalization;
using CsvHelper;

public class CsvManager
{
    private readonly string filepath;
    /// <summary>
    /// Handler for external CSV library, "CsvHelper". Raises an exception if file can't be found.
    /// </summary>
    /// <param name="filepath">Path to .csv file</param>
    /// <exception cref="FileNotFoundException">File can't be found at designated filepath</exception>
    public CsvManager(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException("CSVManager() - CSV filepath not found: ", filepath);
        }
        this.filepath = filepath;
    }

    /// <summary>
    /// Create and return a new IEnum, that spans over every record in the .csv file.<br/><br/>
    /// An IEnum loads only 1 line into memory at a time, whereas a list loads everything.
    /// This can be cast to a list or array anyways, if required.
    /// </summary>
    /// <returns>An IEnumerable collection of Cheep records</returns>
    public IEnumerable<Cheep> GetCheeps()
    {
        // Not using "using" keyword, as an IEnum is returned. Can be changed at some point.
        var csvr = new CsvReader(new StreamReader(filepath), CultureInfo.InvariantCulture);
        return csvr.GetRecords<Cheep>();
    }

    /// <summary>
    /// Writes a single Cheep to the .csv file, using the Cheep record format.
    /// </summary>
    /// <param name="cheep">The Cheep record to enter into the .csv</param>
    public void WriteCheep(Cheep cheep)
    {
        using var csvw = new CsvWriter(new StreamWriter(filepath, append: true), CultureInfo.InvariantCulture);
        csvw.WriteRecord(cheep);
        csvw.NextRecord();
    }

    /// <summary>
    /// Write multiple Cheeps to the .csv using an IEnumerable collection.
    /// </summary>
    /// <param name="cheeps">The IEnumerable collection of Cheep records to enter into the .csv</param>
    public void WriteCheeps(IEnumerable<Cheep> cheeps)
    {
        using var csvw = new CsvWriter(new StreamWriter(filepath, append: true), CultureInfo.InvariantCulture);
        csvw.WriteRecords(cheeps);
        csvw.NextRecord();
    }
}