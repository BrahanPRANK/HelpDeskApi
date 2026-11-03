using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.AsignacionesTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesTickets;

public record ListarAsignacionesTicketsQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>>;

internal sealed class ListarAsignacionesTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarAsignacionesTicketsQueryHandler> _logger) : IRequestHandler<ListarAsignacionesTicketsQuery, ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>> Handle(ListarAsignacionesTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Motivo.Contains(options.Buscar));
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAsignacionTicket).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new AsignacionesTicketsResponseDto
            {
                IdAsignacionTicket = entity.IdAsignacionTicket,
                IdTicket = entity.IdTicket,
                IdEquipo = entity.IdEquipo,
                IdTecnico = entity.IdTecnico,
                IdAsignador = entity.IdAsignador,
                FechaAsignacion = entity.FechaAsignacion,
                FechaFin = entity.FechaFin,
                Motivo = entity.Motivo
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarAsignacionesTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarAsignacionesTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
