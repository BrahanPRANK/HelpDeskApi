using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HelpDesk.Domain.Ports;
using HelpDesk.Infrastructure.Data;
using HelpDesk.Infrastructure.Repositories;
using HelpDesk.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
namespace HelpDesk.Infrastructure.Configuration;

public static class InfrastructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        var connection = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection.");
        var key = config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32) throw new InvalidOperationException("Configure Jwt:Key con al menos 32 bytes en User Secrets o entorno.");
        if (string.IsNullOrWhiteSpace(config["Jwt:Issuer"]) || string.IsNullOrWhiteSpace(config["Jwt:Audience"])) throw new InvalidOperationException("Configure Jwt:Issuer y Jwt:Audience.");
        services.AddDbContext<HelpDeskDbContext>(o => o.UseNpgsql(connection));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, JwtService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                ClockSkew = TimeSpan.Zero,
                NameClaimType = "name",
                RoleClaimType = "role"
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    if (!Guid.TryParse(context.Principal?.FindFirstValue("sub"), out var id)) { context.Fail("Token invalido."); return; }
                    var db = context.HttpContext.RequestServices.GetRequiredService<HelpDeskDbContext>();
                    var user = await db.Usuarios.AsNoTracking().SingleOrDefaultAsync(u => u.IdUsuario == id, context.HttpContext.RequestAborted);
                    if (user is null || !user.Activo) { context.Fail("Usuario inactivo."); return; }
                    var stamp = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(user.ClaveHash)));
                    if (context.Principal?.FindFirstValue("cv") != stamp) { context.Fail("Credenciales actualizadas; inicie sesion."); return; }
                    var roleIds = db.UsuariosRoles.Where(x => x.IdUsuario == id).Select(x => x.IdRol);
                    var roles = await db.Roles.Where(x => roleIds.Contains(x.IdRol)).Select(x => x.Nombre).ToListAsync(context.HttpContext.RequestAborted);
                    var permissions = await db.Roles.Where(x => roleIds.Contains(x.IdRol)).SelectMany(r => r.IdPermiso).Select(p => p.Codigo).Distinct().ToListAsync(context.HttpContext.RequestAborted);
                    var identity = (ClaimsIdentity)context.Principal!.Identity!;
                    foreach (var old in identity.FindAll("role").Concat(identity.FindAll("permiso")).ToList()) identity.RemoveClaim(old);
                    identity.AddClaims(roles.Select(r => new Claim("role", r)));
                    identity.AddClaims(permissions.Select(p => new Claim("permiso", p)));
                }
            };
        });
        services.AddAuthorization(); return services;
    }
}
