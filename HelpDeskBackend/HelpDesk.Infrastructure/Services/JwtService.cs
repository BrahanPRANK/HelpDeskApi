using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDesk.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelpDesk.Infrastructure.Services;

public class JwtService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AccessToken Create(Guid userId, string username, IEnumerable<string> roles, string credentialStamp)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var claims = new List<Claim>
        {
            new("sub", userId.ToString()),
            new("name", username),
            new("jti", Guid.NewGuid().ToString()),
            // Permite rechazar un token anterior cuando cambia la clave del usuario.
            new("cv", credentialStamp)
        };
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
