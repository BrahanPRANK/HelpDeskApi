using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.EquiposSoporte.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EquiposSoporte;

public record ObtenerEquiposSoporteQuery(object[] Keys) : IRequest<ResponseDto<EquiposSoporteResponseDto>>;

internal sealed class ObtenerEquiposSoporteQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerEquiposSoporteQueryHandler> _logger) : IRequestHandler<ObtenerEquiposSoporteQuery, ResponseDto<EquiposSoporteResponseDto>>
{
    public async Task<ResponseDto<EquiposSoporteResponseDto>> Handle(ObtenerEquiposSoporteQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new EquiposSoporteResponseDto { IdEquipo = entity.IdEquipo, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerEquiposSoporteQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerEquiposSoporteQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
