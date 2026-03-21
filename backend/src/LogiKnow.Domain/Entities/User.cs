using Microsoft.AspNetCore.Identity;

namespace LogiKnow.Domain.Entities;

public class User : IdentityUser
{
    public string? FullName { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
