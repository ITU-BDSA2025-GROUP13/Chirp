using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Chirp.Infrastructure
{
    public class ChirpDbContextFactory : IDesignTimeDbContextFactory<ChirpDbContext>
    {
        public ChirpDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChirpDbContext>();
            optionsBuilder.UseSqlite("Data Source=chirp.db"); // Use your actual connection string

            return new ChirpDbContext(optionsBuilder.Options);
        }
    }
}
