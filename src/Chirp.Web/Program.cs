using Chirp.Infrastructure;
using Chirp.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor
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

            appBuilder.Services.AddDbContext<ChirpDbContext>(options =>
                options.UseSqlite(appBuilder.Configuration.GetConnectionString("ChirpDb"))); //Retrieves DB connection from ./appsetings.json
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
    }
}
