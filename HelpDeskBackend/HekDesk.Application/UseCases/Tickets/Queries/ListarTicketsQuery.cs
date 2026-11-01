using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Tickets.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Tickets;

public record ListarTicketsQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<TicketsResponseDto>>>;

internal sealed class ListarTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarTicketsQueryHandler> _logger) : IRequestHandler<ListarTicketsQuery, ResponseDto<PaginaDto<TicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<TicketsResponseDto>>> Handle(ListarTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Tickets>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Asunto.Contains(options.Buscar) || x.Descripcion.Contains(options.Buscar));
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdTicket).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new TicketsResponseDto { IdTicket = entity.IdTicket, Numero = entity.Numero, IdSolicitante = entity.IdSolicitante, IdCreador = entity.IdCreador, IdAreaSolicitante = entity.IdAreaSolicitante, IdServicio = entity.IdServicio, IdEquipoAsignado = entity.IdEquipoAsignado, IdTecnicoAsignado = entity.IdTecnicoAsignado, IdPolitica = entity.IdPolitica, Tipo = entity.Tipo, Asunto = entity.Asunto, Descripcion = entity.Descripcion, Prioridad = entity.Prioridad, Estado = entity.Estado, MotivoEspera = entity.MotivoEspera, Solucion = entity.Solucion, MotivoCancelacion = entity.MotivoCancelacion, MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta, MinutosResolucion = entity.MinutosResolucion, FechaCreacion = entity.FechaCreacion, FechaActualizacion = entity.FechaActualizacion, FechaLimiteRespuesta = entity.FechaLimiteRespuesta, FechaLimiteResolucion = entity.FechaLimiteResolucion, FechaPrimeraRespuesta = entity.FechaPrimeraRespuesta, FechaResolucion = entity.FechaResolucion, FechaCierre = entity.FechaCierre, FechaCancelacion = entity.FechaCancelacion, Version = entity.Version }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
