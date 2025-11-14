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
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789-._@+/ ";
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
            appBuilder.Services.AddScoped<IChirpUserRepository, ChirpUserRepository>();
            appBuilder.Services.AddScoped<ICheepService, CheepService>();
            appBuilder.Services.AddScoped<IChirpUserService, ChirpUserService>();
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
                ChirpDbContext chirpContext = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
                UserManager<ChirpUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ChirpUser>>();
                chirpContext.Database.Migrate();
                DbInitializer.SeedDatabase(chirpContext, userManager);
            }

            app.Run();
        }

        /// <summary>
        /// Returns the path to the database as specified by CHIRPDBPATH
        /// or falls back to /tmp/chirp.db
        /// Also creates the parent directory of the database
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
            }
            return dbPath;
        }
    }
}
