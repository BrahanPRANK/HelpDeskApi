using HelpDesk.Application.Dtos.Autenticacion;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Ports;
namespace HelpDesk.Application.Security;

// Solo inicializacion administrativa por consola; las peticiones HTTP van a sus handlers.
public sealed class AuthService(IUnitOfWork _unitOfWork, IPasswordService passwords)
{
    public async Task Bootstrap(RegistroDto input)
    {
        Validation.Check(input);
        // Restriccion de una sola inicializacion. Se ejecuta por consola, nunca por HTTP.
        var adminRole = (await (_unitOfWork.Repository<Roles>().Query().Where(x => x.Nombre == "administrador")).ToListAsync()).SingleOrDefault()
            ?? throw new BusinessException(409, "Falta el rol administrador.");
        Validation.Require(await (_unitOfWork.Repository<UsuariosRoles>().Query().Where(x => x.IdRol == adminRole.IdRol)).CountAsync() == 0, "Ya existe un administrador; use la API para altas posteriores.", 409);
        var user = MakeUser(input);
        await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().AddAsync(user);
        await _unitOfWork.Repository<UsuariosRoles>().AddAsync(new UsuariosRoles
        {
            IdUsuario = user.IdUsuario,
            IdRol = adminRole.IdRol,
            FechaAsignacion = DateTime.UtcNow
        });
        await _unitOfWork.Complete();
    }
    private Usuarios MakeUser(RegistroDto x) => new()
    {
        IdUsuario = Guid.NewGuid(),
        Nombres = x.Nombres.Trim(),
        Apellidos = x.Apellidos.Trim(),
        NombreUsuario = x.NombreUsuario.Trim(),
        Correo = x.Correo.Trim().ToLowerInvariant(),
        ClaveHash = passwords.Hash(x.Clave),
        Activo = true,
        FechaRegistro = DateTime.UtcNow
    };
}
