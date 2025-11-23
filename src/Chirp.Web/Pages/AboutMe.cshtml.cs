using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO.Compression;
using System.Security.Claims;
using System.Text.Json;

namespace Chirp.Web.Pages
{
    public class AboutMeModel(ICheepService cheepService, IChirpUserService chirpUserService) : PageModel
    {
        private readonly ICheepService _cheepService = cheepService;
        private readonly IChirpUserService _chirpUserService = chirpUserService;
        public string Username { get; private set; } = "";
        public string Email { get; private set; } = "";
        public List<CheepDTO> Cheeps { get; private set; } = [];
        public List<string> FollowedUsers { get; private set; } = [];

        public void OnGet() => UpdateUserInfo();

        private void UpdateUserInfo()
        {
            Username = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            Email = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
            Cheeps = _cheepService.GetAllCheepsFromAuthorName(Username);
            FollowedUsers = _chirpUserService.GetFollowedUsernames(Username);
        }

        public IActionResult OnPostDownloadUserData()
        {
            // Ensure user info is up to date
            UpdateUserInfo();

            // Create an object with user data
            var userData = new
            {
                username = Username,
                email = Email,
                cheeps = Cheeps,
                followedUsers = FollowedUsers
            };

            // Serialize to JSON
            var json = JsonSerializer.Serialize(userData, new JsonSerializerOptions { WriteIndented = true });
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var entry = archive.CreateEntry("user-data.json");
                using var entryStream = entry.Open();
                entryStream.Write(jsonBytes, 0, jsonBytes.Length);
            }

            var zipBytes = memoryStream.ToArray();
            return File(zipBytes, "application/zip", "user-data.zip");
        }
    }
}
