using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.RolesPermisos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RolesPermisos;

public record ObtenerRolesPermisosQuery(object[] Keys) : IRequest<ResponseDto<RolesPermisosResponseDto>>;

internal sealed class ObtenerRolesPermisosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerRolesPermisosQueryHandler> _logger) : IRequestHandler<ObtenerRolesPermisosQuery, ResponseDto<RolesPermisosResponseDto>>
{
    public async Task<ResponseDto<RolesPermisosResponseDto>> Handle(ObtenerRolesPermisosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Dictionary<string, object>>("RolesPermisos").GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new RolesPermisosResponseDto()
            {
                IdRol = (Guid)entity["IdRol"],
                IdPermiso = (Guid)entity["IdPermiso"]
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerRolesPermisosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerRolesPermisosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
