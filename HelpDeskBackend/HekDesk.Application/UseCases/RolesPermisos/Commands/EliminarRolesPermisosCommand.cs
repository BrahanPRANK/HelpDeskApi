using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.RolesPermisos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RolesPermisos;

public record EliminarRolesPermisosCommand(object[] Keys) : IRequest<ResponseDto<OperacionDto>>;

internal sealed class EliminarRolesPermisosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarRolesPermisosCommandHandler> _logger) : IRequestHandler<EliminarRolesPermisosCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarRolesPermisosCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<Dictionary<string, object>>("RolesPermisos").FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            var role=await Required<global::HelpDesk.Domain.Models.Roles>((Guid)entity["IdRol"],ct);
            var permission=await Required<global::HelpDesk.Domain.Models.Permisos>((Guid)entity["IdPermiso"],ct);
            Validation.Require(role.Nombre!="administrador" || permission.Codigo!="usuarios.administrar","Se conserva el permiso base de administracion.",409);
            _unitOfWork.Repository<Dictionary<string, object>>("RolesPermisos").Remove(entity);
            await _unitOfWork.Complete(ct);
            return new(200,new(true),"Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarRolesPermisosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarRolesPermisosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
