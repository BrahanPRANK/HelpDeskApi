using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Tickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Tickets;

public record ObtenerTicketsQuery(object[] Keys) : IRequest<ResponseDto<TicketsResponseDto>>;

internal sealed class ObtenerTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerTicketsQueryHandler> _logger) : IRequestHandler<ObtenerTicketsQuery, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(ObtenerTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Tickets>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new TicketsResponseDto
            {
                IdTicket = entity.IdTicket,
                Numero = entity.Numero,
                IdSolicitante = entity.IdSolicitante,
                IdCreador = entity.IdCreador,
                IdAreaSolicitante = entity.IdAreaSolicitante,
                IdServicio = entity.IdServicio,
                IdEquipoAsignado = entity.IdEquipoAsignado,
                IdTecnicoAsignado = entity.IdTecnicoAsignado,
                IdPolitica = entity.IdPolitica,
                Tipo = entity.Tipo,
                Asunto = entity.Asunto,
                Descripcion = entity.Descripcion,
                Prioridad = entity.Prioridad,
                Estado = entity.Estado,
                MotivoEspera = entity.MotivoEspera,
                Solucion = entity.Solucion,
                MotivoCancelacion = entity.MotivoCancelacion,
                MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta,
                MinutosResolucion = entity.MinutosResolucion,
                FechaCreacion = entity.FechaCreacion,
                FechaActualizacion = entity.FechaActualizacion,
                FechaLimiteRespuesta = entity.FechaLimiteRespuesta,
                FechaLimiteResolucion = entity.FechaLimiteResolucion,
                FechaPrimeraRespuesta = entity.FechaPrimeraRespuesta,
                FechaResolucion = entity.FechaResolucion,
                FechaCierre = entity.FechaCierre,
                FechaCancelacion = entity.FechaCancelacion,
                Version = entity.Version
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
