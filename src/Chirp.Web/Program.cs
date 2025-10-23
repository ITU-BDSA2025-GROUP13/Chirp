using Chirp.Infrastructure;
using Chirp.Domain;
using Chirp.Razor;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Builds the web application
            WebApplicationBuilder appBuilder = WebApplication.CreateBuilder(args);

            appBuilder.Services.AddRazorPages();

            appBuilder.Services.AddScoped<ICheepRepository, CheepRepository>();
            appBuilder.Services.AddScoped<ICheepService, CheepService>();
            appBuilder.Services.AddScoped<IChirpDbContext>(provider =>
                provider.GetRequiredService<ChirpDbContext>());

            string dbPath = GetDBPathFromEnv();
            appBuilder.Services.AddDbContext<ChirpDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
            WebApplication app = appBuilder.Build();

            //Sets up middleware pipelines
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapRazorPages();

            //Database initialization
            using (IServiceScope scope = app.Services.CreateScope())
            {
                ChirpDbContext db = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
                try
                {
                    if (db.Database.EnsureCreated())
                    {
                        Console.WriteLine("Database not found, creating one based on seed...");
                        DbInitializer.SeedDatabase(db);
                    }
                }
                catch (Microsoft.Data.Sqlite.SqliteException err)
                {
                    Console.WriteLine(err);
                }

            }

            app.Run();
        }

        /// <summary>
        /// Returns the path to the database as specified by CHIRPDBPATH
        /// or falls back to /tmp/chirp.db
        /// </summary>
        /// <returns>
        /// $CHIRPDBPATH or $TMPDIR/chirp.db
        /// </returns>
        private static string GetDBPathFromEnv()
        {
            string? dbPathFromEnv = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            string dbPath = string.IsNullOrEmpty(dbPathFromEnv)
                ? $"{Path.GetTempPath()}/chirp.db"
                : dbPathFromEnv;

            // Ensure directory and file exist
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
            return dbPath;
        }
    }
}
