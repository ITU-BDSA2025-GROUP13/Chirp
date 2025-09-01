using System.Text.RegularExpressions;

class Program
{
    const string CSV = "assets/chirp_cli_db.csv";

    static int Main(string[] args)
    {
        if (args[0] == "read")
        {
            read();
        }
        else if (args[0] == "cheep")
        {
            string message = args[1];
            write(message);
        }
        return 0;
    }

    /// <summary>
    /// Reads out all chirps in the csv file
    /// </summary>
    static private void read()
    {
        var input = File.ReadAllLines(CSV);

        for (int i = 1; i < input.Length; i++)
        {
            var line = Regex.Split(input[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            string userMessage = line[1].Replace("\"", "");
            DateTime dateFormat = DateTime.UnixEpoch.AddSeconds(long.Parse(line[2]) + 7200);
            string dateString = dateFormat.Month.ToString("D2") + "/" + dateFormat.Day.ToString("D2") + "/" +
                               dateFormat.Year.ToString().Substring(2, 2) + " " + dateFormat.ToString("HH:mm:ss");

            Console.WriteLine(line[0] + " @ " + dateString + ": " + userMessage);
        }
    }

    /// <summary>
    /// Writes the message to the csv file
    /// Username is the currently logged in user
    /// </summary>
    /// <param name="message"> The message to write to the csv file </param>
    static private void write(string message)
    {
        // Get metadata and cheep
        string user = Environment.UserName;
        long epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Format and save
        var data = user + ',' + '"' + message + '"' + ',' + epoch + '\n';
        File.AppendAllText(CSV, data);

    }
}
