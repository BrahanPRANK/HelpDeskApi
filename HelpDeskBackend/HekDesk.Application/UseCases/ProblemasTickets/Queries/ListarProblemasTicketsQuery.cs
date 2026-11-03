using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ProblemasTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ProblemasTickets;

public record ListarProblemasTicketsQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>>;

internal sealed class ListarProblemasTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarProblemasTicketsQuery, ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>>
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
            return new ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>(200, new PaginaDto<ProblemasTicketsResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
