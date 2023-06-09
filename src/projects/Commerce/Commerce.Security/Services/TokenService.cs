using System.Text;
using System.Security.Claims;
using Commerce.Security.Models;
using Commerce.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Commerce.Security.Helpers;
using Commerce.Security.Helpers.Exceptions;
using Microsoft.Extensions.Configuration;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Commerce.Security.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration) => _configuration = configuration;

    public string GenerateToken(User user)
    {
        var key = EnvironmentVariable.GetRequiredEnvironmentVariable("JWT_KEY");
        var encodedKey = Encoding.UTF8.GetBytes(key);

        if (encodedKey.Length < 32)
        {
            throw new InvalidTokenException("JWT Key must be at least 256 bits long.");
        }

        var securityKey = new SymmetricSecurityKey(encodedKey);
        var tokenDescriptor = CreateTokenDescriptor(user, securityKey);
        
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var previewToken = jwtTokenHandler.CreateToken(tokenDescriptor);
        var token = jwtTokenHandler.WriteToken(previewToken);
        return token;
    }

    public string GenerateEmailToken(User user)
    {
        var key = EnvironmentVariable.GetRequiredEnvironmentVariable("EMAIL_KEY");
        var encodedKey = Encoding.UTF8.GetBytes(key);

        if (encodedKey.Length < 32)
        {
            throw new InvalidTokenException("The Email Key must be at least 256 bits long.");
        }

        var securityKey = new SymmetricSecurityKey(encodedKey);
        var tokenDescriptor = CreateEmailTokenDescriptor(user, securityKey);

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var emailToken = jwtTokenHandler.CreateToken(tokenDescriptor);
        var token = jwtTokenHandler.WriteToken(emailToken);
        return token;
    }

    public string ExtractEmailClaim(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secToken = tokenHandler.ReadJwtToken(token);

        string? email = secToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        if (email is null)
        {
            throw new InvalidTokenException("O token não é válido.");
        }
        
        return email;
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(User user, SymmetricSecurityKey securityKey)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
            Expires = DateTime.UtcNow.AddMinutes(10),
            Audience = EnvironmentVariable.GetRequiredEnvironmentVariable("JWT_AUDIENCE"),
            Issuer = EnvironmentVariable.GetRequiredEnvironmentVariable("JWT_ISSUER")
        };

        return tokenDescriptor;
    }

    private SecurityTokenDescriptor CreateEmailTokenDescriptor(User user, SymmetricSecurityKey securityKey)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("email", user.EmailAddress),
            }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        return tokenDescriptor;
    }
}