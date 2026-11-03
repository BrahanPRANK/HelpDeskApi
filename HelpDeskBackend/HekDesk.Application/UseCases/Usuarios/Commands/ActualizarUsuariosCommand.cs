using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Usuarios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios;

public record ActualizarUsuariosCommand(object[] Keys, UsuariosInputDto Datos) : IRequest<ResponseDto<UsuariosResponseDto>>;

internal sealed class ActualizarUsuariosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, IPasswordService passwords, ILogger<ActualizarUsuariosCommandHandler> _logger) : IRequestHandler<ActualizarUsuariosCommand, ResponseDto<UsuariosResponseDto>>
{
    public async Task<ResponseDto<UsuariosResponseDto>> Handle(ActualizarUsuariosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            if (!input.Activo) await EnsureNotLastAdmin(entity.IdUsuario);
            entity.IdArea = input.IdArea;
            entity.Nombres = input.Nombres;
            entity.Apellidos = input.Apellidos;
            entity.NombreUsuario = input.NombreUsuario;
            entity.Correo = input.Correo;
            entity.Telefono = input.Telefono;
            entity.Activo = input.Activo;
            var userInput = input;
            entity.NombreUsuario = entity.NombreUsuario.Trim();
            entity.Correo = entity.Correo.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(userInput.Clave)) entity.ClaveHash = passwords.Hash(userInput.Clave);

            await _unitOfWork.Complete();
            return new(200, new UsuariosResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdArea = entity.IdArea,
                Nombres = entity.Nombres,
                Apellidos = entity.Apellidos,
                NombreUsuario = entity.NombreUsuario,
                Correo = entity.Correo,
                Telefono = entity.Telefono,
                Activo = entity.Activo,
                FechaRegistro = entity.FechaRegistro
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarUsuariosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarUsuariosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }
    private async Task EnsureNotLastAdmin(Guid userId)
    {
        var roleIds = _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query().Where(r => r.Nombre == "administrador").Select(r => r.IdRol);
        if (!_unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Any(r => r.IdUsuario == userId && roleIds.Contains(r.IdRol))) return;
        var others = _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Where(r => roleIds.Contains(r.IdRol) && r.IdUsuario != userId).Select(r => r.IdUsuario);
        Validation.Require(await (_unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().Query().Where(u => u.Activo && others.Contains(u.IdUsuario))).CountAsync() > 0,
            "Debe conservarse al menos un administrador activo.", 409);
    }

}
