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
            optionsBuilder.UseSqlite("Data Source=../../data/chirp.db");

            return new ChirpDbContext(optionsBuilder.Options);
        }
    }
}
