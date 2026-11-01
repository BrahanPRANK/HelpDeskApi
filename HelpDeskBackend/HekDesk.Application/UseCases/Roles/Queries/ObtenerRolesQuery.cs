using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Roles.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Roles;

public record ObtenerRolesQuery(object[] Keys) : IRequest<ResponseDto<RolesResponseDto>>;

internal sealed class ObtenerRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerRolesQueryHandler> _logger) : IRequestHandler<ObtenerRolesQuery, ResponseDto<RolesResponseDto>>
{
    public async Task<ResponseDto<RolesResponseDto>> Handle(ObtenerRolesQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new RolesResponseDto { IdRol = entity.IdRol, Nombre = entity.Nombre, Descripcion = entity.Descripcion });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerRolesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerRolesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
