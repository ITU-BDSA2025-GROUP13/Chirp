using Chirp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.DatabaseContext
{
    /// <summary>
    /// Database context for the Chirp application, managing Users and Messages entities.
    /// </summary>
    /// <param name="options">Configuration options for the database context.</param>
    public class ChirpDbContext : DbContext, IChirpDbContext
    {
        public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the collection of users in the database.
        /// </summary>
        public DbSet<Author> Authors { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of messages in the database.
        /// </summary>
        public DbSet<Cheep> Cheeps { get; set; } = null!;

        /// <summary>
        /// Configures the database schema and entity relationships.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(u => u.AuthorId);
                entity.Property(u => u.AuthorId).HasColumnName("user_id").ValueGeneratedOnAdd(); // Autoincrement
                entity.Property(u => u.Name).HasColumnName("username");
                entity.Property(u => u.Email).HasColumnName("email");
            });

            modelBuilder.Entity<Cheep>(entity =>
            {
                entity.ToTable("message");
                entity.HasKey(m => m.CheepId);
                entity.Property(m => m.CheepId).HasColumnName("message_id").ValueGeneratedOnAdd(); // Autoincrement
                entity.Property(m => m.AuthorId).HasColumnName("author_id");
                entity.Property(m => m.Text).HasColumnName("text");
                entity.ToTable(t => t.HasCheckConstraint("length_constraint", "length(text) <= 160"));
                entity.Property(m => m.TimeStamp).HasColumnName("pub_date");
            });
        }
    }
}
