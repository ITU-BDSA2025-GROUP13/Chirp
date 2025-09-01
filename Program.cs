
using System.Text.RegularExpressions;

const string CSV = "assets/chirp_cli_db.csv";

if (args[0] == "read")
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
else if (args[0] == "cheep")
{
    // Get metadata and cheep
    string user = Environment.UserName;
    long epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    string message = args[1];

    // Format and save
    var data = user + ',' + '"' + message + '"' + ',' + epoch + '\n';
    File.AppendAllText(CSV, data);
}
