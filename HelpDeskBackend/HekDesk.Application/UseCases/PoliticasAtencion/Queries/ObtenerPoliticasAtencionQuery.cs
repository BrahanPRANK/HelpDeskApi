using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.PoliticasAtencion.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PoliticasAtencion;

public record ObtenerPoliticasAtencionQuery(object[] Keys) : IRequest<ResponseDto<PoliticasAtencionResponseDto>>;

internal sealed class ObtenerPoliticasAtencionQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerPoliticasAtencionQueryHandler> _logger) : IRequestHandler<ObtenerPoliticasAtencionQuery, ResponseDto<PoliticasAtencionResponseDto>>
{
    public async Task<ResponseDto<PoliticasAtencionResponseDto>> Handle(ObtenerPoliticasAtencionQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new PoliticasAtencionResponseDto { IdPolitica = entity.IdPolitica, Prioridad = entity.Prioridad, MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta, MinutosResolucion = entity.MinutosResolucion, Activo = entity.Activo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerPoliticasAtencionQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerPoliticasAtencionQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
