using Commerce.Security.Models;
using Commerce.Security.Utils;

namespace Commerce.Security.Interfaces;

public interface ITokenRequest
{
    string GenerateToken(User user);
}