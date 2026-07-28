using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Services;

public sealed class TokenService(IConfiguration configuration)
{
    public (string Token, DateTime ExpiresAt) CreateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret is not configured.");
        var expiresAt = DateTime.UtcNow.AddMinutes(configuration.GetValue("Jwt:ExpiryMinutes", 60));
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.Email!)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
