using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDesk.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace HelpDesk.Infrastructure.Services;
public sealed class JwtService(IConfiguration config):ITokenService
{
    public AccessToken Create(Guid userId,string username,IEnumerable<string> roles,string credentialStamp)
    {
        var expiry=DateTime.UtcNow.AddMinutes(30);
        var claims=new List<Claim> {new("sub",userId.ToString()),new("name",username),new("cv",credentialStamp),new("jti",Guid.NewGuid().ToString())};
        claims.AddRange(roles.Select(r=>new Claim("role",r)));
        var token=new JwtSecurityToken(issuer:config["Jwt:Issuer"],audience:config["Jwt:Audience"],claims:claims,
            notBefore:DateTime.UtcNow,expires:expiry,signingCredentials:new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(token),expiry);
    }
}
