using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public record ObtenerUsuariosRolesQuery(object[] Keys) : IRequest<ResponseDto<UsuariosRolesResponseDto>>;

internal sealed class ObtenerUsuariosRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerUsuariosRolesQueryHandler> _logger) : IRequestHandler<ObtenerUsuariosRolesQuery, ResponseDto<UsuariosRolesResponseDto>>
{
    public async Task<ResponseDto<UsuariosRolesResponseDto>> Handle(ObtenerUsuariosRolesQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new UsuariosRolesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdRol = entity.IdRol,
                FechaAsignacion = entity.FechaAsignacion
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerUsuariosRolesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerUsuariosRolesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
