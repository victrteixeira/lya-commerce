using System.Text;
using System.Security.Claims;
using Commerce.Security.Utils;
using Commerce.Security.Models;
using Commerce.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Commerce.Security.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var encodedKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        if (encodedKey.Length < 32)
            throw new InvalidTokenException("JWT Key must be at least 256 bits long");

        var securityKey = new SymmetricSecurityKey(encodedKey);
        var tokenDescriptor = CreateTokenDescriptor(user, securityKey);
        
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var previewToken = jwtTokenHandler.CreateToken(tokenDescriptor);
        var token = jwtTokenHandler.WriteToken(previewToken);
        return token;
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(User user, SymmetricSecurityKey securityKey)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName)
            }),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ValidForMinutes"])),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
            Audience = _configuration["Jwt:Audience"],
            Issuer = _configuration["Jwt:Issuer"]
        };

        return tokenDescriptor;
    }
}