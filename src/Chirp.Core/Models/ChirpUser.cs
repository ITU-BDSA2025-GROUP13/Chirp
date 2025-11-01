using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Models;

public class ChirpUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
