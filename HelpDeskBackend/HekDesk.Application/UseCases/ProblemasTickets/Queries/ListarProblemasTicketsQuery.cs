using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.ProblemasTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ProblemasTickets;

public record ListarProblemasTicketsQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>>;

internal sealed class ListarProblemasTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarProblemasTicketsQueryHandler> _logger) : IRequestHandler<ListarProblemasTicketsQuery, ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>> Handle(ListarProblemasTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x["IdProblema"]).ThenBy(x => x["IdTicket"]).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new ProblemasTicketsResponseDto()
            {
                IdProblema = (Guid)entity["IdProblema"],
                IdTicket = (Guid)entity["IdTicket"]
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarProblemasTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarProblemasTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
