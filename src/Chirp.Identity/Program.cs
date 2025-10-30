using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Chirp.Identity.Data;
using Chirp.Infrastructure.DatabaseContext;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<IChirpDbContext>(provider =>
    provider.GetRequiredService<ChirpDbContext>());

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

builder.Services.AddDbContext<ChirpDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));


builder.Services.AddRazorPages();

var app = builder.Build();

//Chirp Database initialization
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


// Identity Database initialization
using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (!db.Database.CanConnect())
        {
            Console.WriteLine("Identity database not found, creating one...");
            db.Database.Migrate();

        }
    }
    catch (Microsoft.Data.Sqlite.SqliteException err)
    {
        Console.WriteLine(err);
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
