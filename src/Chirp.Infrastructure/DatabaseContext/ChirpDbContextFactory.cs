using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Chirp.Infrastructure
{
    /// <summary>
    /// Used by EFCore to perform migrations
    /// </summary>
    public class ChirpDbContextFactory : IDesignTimeDbContextFactory<ChirpDbContext>
    {
        public ChirpDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChirpDbContext>();
            string? dbPathFromEnv = Environment.GetEnvironmentVariable("DB_PATH");
            string dbPath = string.IsNullOrEmpty(dbPathFromEnv) ? $"{Path.GetTempPath()}/chirp.db" : dbPathFromEnv;

            if (!File.Exists(dbPath))
            {
                string? dbDir = Path.GetDirectoryName(dbPath);
                // Must check if dbDir is null or empty, else compiler warns
                if (!string.IsNullOrEmpty(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                File.Create(dbPath);
            }

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            return new ChirpDbContext(optionsBuilder.Options);
        }
    }
}
