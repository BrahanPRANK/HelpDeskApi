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

public record CrearRolesPermisosCommand(RolesPermisosInputDto Datos) : IRequest<ResponseDto<RolesPermisosResponseDto>>;

internal sealed class CrearRolesPermisosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearRolesPermisosCommandHandler> _logger) : IRequestHandler<CrearRolesPermisosCommand, ResponseDto<RolesPermisosResponseDto>>
{
    public async Task<ResponseDto<RolesPermisosResponseDto>> Handle(CrearRolesPermisosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new Dictionary<string, object>() { ["IdRol"] = input.IdRol, ["IdPermiso"] = input.IdPermiso };

            await _unitOfWork.Repository<Dictionary<string, object>>("RolesPermisos").AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new RolesPermisosResponseDto()
            {
                IdRol = (Guid)entity["IdRol"],
                IdPermiso = (Guid)entity["IdPermiso"]
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearRolesPermisosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearRolesPermisosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
