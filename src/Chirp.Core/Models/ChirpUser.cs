using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Models;

/// <summary>
/// Empty user that can be extended to add required fields
/// </summary>
public class ChirpUser : IdentityUser
{
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
}
