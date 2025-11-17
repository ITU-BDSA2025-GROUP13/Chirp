namespace Chirp.Integration.Tests.E2E;

using System.Diagnostics;

public class ChirpEndToEndPlaywrightFixture : IDisposable
{
    private readonly string dbPath = $"{Path.GetTempPath()}/chirp/e2eTest.db";
    public readonly string _baseUrl = "http://localhost:5273";
    private readonly Process _serverProcess;

    public ChirpEndToEndPlaywrightFixture()
    {
        var projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "src", "Chirp.Web"));

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --no-build --urls {_baseUrl}",
            WorkingDirectory = projectDir,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.Environment["CHIRPDBPATH"] = dbPath;
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Testing";

        _serverProcess = Process.Start(startInfo) ?? throw new Exception("Failed to start Chirp.Web");

        // HACK: Wait for Chirp.Web to be ready
        Thread.Sleep(5000);
    }

    /// <summary>
    /// Stop Chirp.Web and remove the test database
    /// </summary>
    public void Dispose()
    {
        try
        {
            _serverProcess.Kill(entireProcessTree: true);
            _serverProcess.WaitForExit();
            _serverProcess.Dispose();
        }
        catch { }

        File.Delete(dbPath);
    }
}
