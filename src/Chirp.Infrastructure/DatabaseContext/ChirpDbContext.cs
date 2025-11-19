using Chirp.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<ChirpUser> ChirpUsers { get; set; } = null!;

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
                entity.HasKey(c => c.CheepId);
                entity.Property(c => c.CheepId).HasColumnName("message_id").ValueGeneratedOnAdd(); // Autoincrement
                entity.Property(c => c.AuthorId).HasColumnName("author_id");
                entity.Property(c => c.Text).HasColumnName("text");
                entity.ToTable(t => t.HasCheckConstraint("length_constraint", "length(text) <= 160"));
                entity.Property(c => c.TimeStamp).HasColumnName("pub_date");

                entity.HasOne(c => c.Author)
                      .WithMany(c => c.Cheeps)
                      .HasForeignKey(c => c.AuthorId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity
                    .HasMany(c => c.Replies)
                    .WithOne(c => c.ParentCheep)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasMany(u => u.UsersWhoLiked)
                      .WithMany(u => u.LikedCheeps);
            });

            modelBuilder.Entity<ChirpUser>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.HasIndex(u => u.UserName).IsUnique();
                // Both FollowsList and FollowedByList are both required, otherwise Migration will fail for some reason
                entity.HasMany(u => u.FollowsList).WithMany(u => u.FollowedByList)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserFollowedByList",
                        j => j.HasOne<ChirpUser>().WithMany()
                            .HasForeignKey("AId").OnDelete(DeleteBehavior.NoAction),
                        j => j.HasOne<ChirpUser>().WithMany()
                            .HasForeignKey("BId").OnDelete(DeleteBehavior.NoAction)
                    );

                entity.HasMany(u => u.LikedCheeps)
                      .WithMany(u => u.UsersWhoLiked);
            });
        }
    }
}
