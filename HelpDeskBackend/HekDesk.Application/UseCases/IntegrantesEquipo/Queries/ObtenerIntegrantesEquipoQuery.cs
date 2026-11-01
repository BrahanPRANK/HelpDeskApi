using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.IntegrantesEquipo.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo;

public record ObtenerIntegrantesEquipoQuery(object[] Keys) : IRequest<ResponseDto<IntegrantesEquipoResponseDto>>;

internal sealed class ObtenerIntegrantesEquipoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerIntegrantesEquipoQueryHandler> _logger) : IRequestHandler<ObtenerIntegrantesEquipoQuery, ResponseDto<IntegrantesEquipoResponseDto>>
{
    public async Task<ResponseDto<IntegrantesEquipoResponseDto>> Handle(ObtenerIntegrantesEquipoQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new IntegrantesEquipoResponseDto { IdEquipo = entity.IdEquipo, IdUsuario = entity.IdUsuario, EsCoordinador = entity.EsCoordinador, Disponible = entity.Disponible, Activo = entity.Activo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerIntegrantesEquipoQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerIntegrantesEquipoQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
