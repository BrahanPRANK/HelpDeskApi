using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CambiosTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CambiosTickets;

public record ListarCambiosTicketsQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<CambiosTicketsResponseDto>>>;

internal sealed class ListarCambiosTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarCambiosTicketsQuery, ResponseDto<PaginaDto<CambiosTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CambiosTicketsResponseDto>>> Handle(ListarCambiosTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<Dictionary<string, object>>("CambiosTickets").Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x["IdCambio"]).ThenBy(x => x["IdTicket"]).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new CambiosTicketsResponseDto()
            {
                IdCambio = (Guid)entity["IdCambio"],
                IdTicket = (Guid)entity["IdTicket"]
            })).ToListAsync();
            return new ResponseDto<PaginaDto<CambiosTicketsResponseDto>>(200, new PaginaDto<CambiosTicketsResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<CambiosTicketsResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<CambiosTicketsResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
