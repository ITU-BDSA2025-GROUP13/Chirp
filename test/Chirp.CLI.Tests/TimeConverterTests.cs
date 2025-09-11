namespace Chirp.CLI.Tests;

public class TimeConverterTests
{
    [Fact]
    public void ConvertUnixTimestamp_ReturnsCorrectReadableTime()
    {
        long unixTimestamp = 1609459200;
        string expected = "01/01/21 00:00:00";
        string actual = TimeConverter.ToReadable(unixTimestamp);
        Assert.Equal(expected, actual);
        Console.WriteLine("YEAH");
    }
}
