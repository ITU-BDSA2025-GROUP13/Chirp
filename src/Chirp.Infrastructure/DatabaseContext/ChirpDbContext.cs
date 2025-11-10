using Chirp.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Chirp.Infrastructure.DatabaseContext
{
    /// <summary>
    /// Database context for the Chirp application, managing Users and Messages entities.
    /// </summary>
    /// <param name="options">Configuration options for the database context.</param>
    public class ChirpDbContext : IdentityDbContext<ChirpUser>, IChirpDbContext
    {
        public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) { }

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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cheep>(entity =>
            {
                entity.ToTable("message");
                entity.HasKey(m => m.CheepId);
                entity.Property(m => m.CheepId).HasColumnName("message_id").ValueGeneratedOnAdd(); // Autoincrement
                entity.Property(m => m.AuthorId).HasColumnName("author_id");
                entity.Property(m => m.Text).HasColumnName("text");
                entity.ToTable(t => t.HasCheckConstraint("length_constraint", "length(text) <= 160"));
                entity.Property(m => m.TimeStamp).HasColumnName("pub_date");

                entity.HasOne(m => m.Author)
                      .WithMany(m => m.Cheeps)
                      .HasForeignKey(m => m.AuthorId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ChirpUser>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.HasIndex(u => u.UserName).IsUnique();
            });
        }
    }
}
