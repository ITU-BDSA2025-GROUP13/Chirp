using Chirp.Core.Models;
using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Chirp.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Builds the web application
            WebApplicationBuilder appBuilder = WebApplication.CreateBuilder(args);

            string dbPath = GetDBPathFromEnv();
            appBuilder.Services.AddDbContext<ChirpDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            appBuilder.Services.AddDefaultIdentity<ChirpUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // TODO: Consider enabling email verification
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ChirpDbContext>();

            appBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
            })
            .AddGitHub(options =>
            {
                options.ClientId = appBuilder.Configuration["authentication:github:clientId"]!;
                options.ClientSecret = appBuilder.Configuration["authentication:github:clientSecret"]!;
                options.CallbackPath = "/signin-github";
                options.Scope.Add("user:email");
            });

            appBuilder.Services.AddSession();
            appBuilder.Services.AddRazorPages();

            appBuilder.Services.AddScoped<ICheepRepository, CheepRepository>();
            appBuilder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            appBuilder.Services.AddScoped<ICheepService, CheepService>();
            appBuilder.Services.AddScoped<IChirpDbContext>(provider =>
                provider.GetRequiredService<ChirpDbContext>());

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages();

            // Database seeding & migration
            using (IServiceScope scope = app.Services.CreateScope())
            {
                ChirpDbContext db = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
                db.Database.Migrate();
                DbInitializer.SeedDatabase(db);
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
