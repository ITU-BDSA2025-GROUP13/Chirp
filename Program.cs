
using System.Text.RegularExpressions;


if (args[0] == "read")
{
    var input = File.ReadAllLines("assets/chirp_cli_db.csv");

    for (int i = 1; i < input.Length; i++)
    {
        var line = Regex.Split(input[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");W
    
        string userMessage = line[1].Replace("\"", "");
        DateTime dateFormat = DateTime.UnixEpoch.AddSeconds(long.Parse(line[2]) + 7200);
        string dateString = dateFormat.Month.ToString("D2") + "/" + dateFormat.Day.ToString("D2") + "/" +
                           dateFormat.Year.ToString().Substring(2, 2) + " " + dateFormat.ToString("HH:mm:ss");
    
        Console.WriteLine(line[0] + " @ " + dateString + ": " + userMessage);
    }    
}
