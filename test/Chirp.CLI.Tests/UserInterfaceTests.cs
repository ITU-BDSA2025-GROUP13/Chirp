using Chirp.CLI.Models;

namespace Chirp.CLI.Tests
{
    public class UserInterfaceTests
    {
        [Fact]
        public void PrintCheeps_WritesExpectedOutputToConsole()
        {
            var testCheeps = new List<Cheep> {
                new Cheep("Auther1", "This is the 1st test cheep.", 1690891760),
                new Cheep("Auther2", "This is the 2nd test cheep.", 1690979858),
                new Cheep("Auther3", "This is the 3rd test cheep.", 1690981487)
            };

            var originalOut = Console.Out;
            var testOut = new StringWriter();
            Console.SetOut(testOut);

            try
            {
                UserInterface.PrintCheeps(testCheeps);
                string actualOutput = testOut.ToString();
                string expectedOutput =
                    "Auther1 @ 01/08/23 14:09:20: This is the 1st test cheep." + Environment.NewLine +
                    "Auther2 @ 02/08/23 14:37:38: This is the 2nd test cheep." + Environment.NewLine +
                    "Auther3 @ 02/08/23 15:04:47: This is the 3rd test cheep." + Environment.NewLine;

                Assert.Equal(expectedOutput, actualOutput);
            }
            finally
            {
                Console.SetOut(originalOut);
                testOut.Dispose();
            }
        }
    }
}
