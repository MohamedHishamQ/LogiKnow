using LogiKnow.Domain.Entities;

namespace LogiKnow.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, IList<string> roles);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string token);
}
