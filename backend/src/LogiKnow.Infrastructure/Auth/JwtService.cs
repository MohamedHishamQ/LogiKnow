using LogiKnow.Application.Interfaces;
using LogiKnow.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogiKnow.Infrastructure.Auth;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config) => _config = config;

    public string GenerateToken(User user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"] ?? "DEFAULT_SECRET_KEY_REPLACE_ME_256BIT_MINIMUM!!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.FullName ?? user.Email ?? ""),
            new("preferred_language", user.PreferredLanguage)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var expiryMinutes = int.TryParse(_config["JwtSettings:ExpiryMinutes"], out var em) ? em : 60;

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"] ?? "logiknow-api",
            audience: _config["JwtSettings:Audience"] ?? "logiknow-client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public bool ValidateRefreshToken(string token)
    {
        // In production, validate against stored refresh tokens in DB
        return !string.IsNullOrEmpty(token);
    }
}
