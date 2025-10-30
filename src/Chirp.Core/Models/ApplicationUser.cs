using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}