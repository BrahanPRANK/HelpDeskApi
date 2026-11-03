using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TicketsArticulos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsArticulos;

public record ListarTicketsArticulosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<TicketsArticulosResponseDto>>>;

internal sealed class ListarTicketsArticulosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarTicketsArticulosQueryHandler> _logger) : IRequestHandler<ListarTicketsArticulosQuery, ResponseDto<PaginaDto<TicketsArticulosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<TicketsArticulosResponseDto>>> Handle(ListarTicketsArticulosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.TicketsArticulos>().Query());
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdTicket).ThenBy(x => x.IdArticulo).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new TicketsArticulosResponseDto
            {
                IdTicket = entity.IdTicket,
                IdArticulo = entity.IdArticulo,
                IdUsuario = entity.IdUsuario,
                FechaVinculacion = entity.FechaVinculacion
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarTicketsArticulosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarTicketsArticulosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
