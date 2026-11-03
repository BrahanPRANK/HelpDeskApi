using HelpDesk.Application.Dtos.Common;
using HelpDesk.Application.Dtos.Autenticacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.Security;

public record RegistrarCommand(RegistroDto Datos) : IRequest<ResponseDto<UsuarioRegistradoDto>>;
internal sealed class RegistrarCommandHandler(IUnitOfWork _unitOfWork, IPasswordService passwords, ILogger<RegistrarCommandHandler> _logger) : IRequestHandler<RegistrarCommand, ResponseDto<UsuarioRegistradoDto>>
{
    public async Task<ResponseDto<UsuarioRegistradoDto>> Handle(RegistrarCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;

            Validation.Check(input);
            var employee = (await (_unitOfWork.Repository<Roles>().Query().Where(x => x.Nombre == "empleado")).ToListAsync()).SingleOrDefault()
            ?? throw new BusinessException(409, "Falta el rol empleado del catalogo inicial.");
            var user = MakeUser(input);
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().AddAsync(user);
            await _unitOfWork.Repository<UsuariosRoles>().AddAsync(new UsuariosRoles
            {
                IdUsuario = user.IdUsuario,
                IdRol = employee.IdRol,
                FechaAsignacion = DateTime.UtcNow
            });
            await _unitOfWork.Complete();
            return new(201, new(user.IdUsuario, user.NombreUsuario), "Cuenta creada con rol empleado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en RegistrarCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar RegistrarCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
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
