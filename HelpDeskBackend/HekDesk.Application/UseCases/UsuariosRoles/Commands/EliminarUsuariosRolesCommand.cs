using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public record EliminarUsuariosRolesCommand(object[] Keys) : IRequest<ResponseDto<OperacionDto>>;

internal sealed class EliminarUsuariosRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarUsuariosRolesCommandHandler> _logger) : IRequestHandler<EliminarUsuariosRolesCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarUsuariosRolesCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            await EnsureNotLastAdmin(entity.IdUsuario);
            _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>(null).Delete(entity);
            await _unitOfWork.Complete();
            return new(200, new(true), "Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarUsuariosRolesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarUsuariosRolesCommandHandler.");
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
