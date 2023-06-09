using Commerce.Security.Models;

namespace Commerce.Security.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateEmailToken(User user);
    string ExtractEmailClaim(string token);
}