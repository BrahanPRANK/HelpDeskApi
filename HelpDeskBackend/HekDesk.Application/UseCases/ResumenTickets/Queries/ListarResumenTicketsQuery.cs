using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ResumenTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ResumenTickets;

public record ListarResumenTicketsQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<ResumenTicketsResponseDto>>>;

internal sealed class ListarResumenTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarResumenTicketsQueryHandler> _logger) : IRequestHandler<ListarResumenTicketsQuery, ResponseDto<PaginaDto<ResumenTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ResumenTicketsResponseDto>>> Handle(ListarResumenTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.ResumenTickets>().Query());
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.Numero).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new ResumenTicketsResponseDto
            {
                IdTicket = entity.IdTicket,
                Numero = entity.Numero,
                Asunto = entity.Asunto,
                Tipo = entity.Tipo,
                Estado = entity.Estado,
                Prioridad = entity.Prioridad,
                IdSolicitante = entity.IdSolicitante,
                IdAreaSolicitante = entity.IdAreaSolicitante,
                IdEquipoAsignado = entity.IdEquipoAsignado,
                IdTecnicoAsignado = entity.IdTecnicoAsignado,
                FechaCreacion = entity.FechaCreacion,
                FechaLimiteRespuesta = entity.FechaLimiteRespuesta,
                FechaLimiteResolucion = entity.FechaLimiteResolucion,
                FechaPrimeraRespuesta = entity.FechaPrimeraRespuesta,
                FechaResolucion = entity.FechaResolucion,
                FechaCierre = entity.FechaCierre,
                MinutosHastaResolucion = entity.MinutosHastaResolucion,
                RespuestaEnPlazo = entity.RespuestaEnPlazo,
                ResolucionEnPlazo = entity.ResolucionEnPlazo,
                Calificacion = entity.Calificacion
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarResumenTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarResumenTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
