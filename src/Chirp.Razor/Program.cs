using Chirp.Infrastructure;
using Chirp.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Register ChirpDbContext with connection string
            builder.Services.AddDbContext<ChirpDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("ChirpDb")));

            // Register IChirpDbContext to resolve to ChirpDbContext
            builder.Services.AddScoped<IChirpDbContext>(provider => provider.GetRequiredService<ChirpDbContext>());

            builder.Services.AddScoped<ICheepRepository, CheepRepository>();
            builder.Services.AddScoped<ICheepService, CheepService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
                try
                {
                    if (db.Database.EnsureCreated()) // Replace Cheeps with your main DbSet
                    {
                        Console.WriteLine("Database not found, performing migrations...");
                        DbInitializer.SeedDatabase(db);
                        //db.Database.Migrate();
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
