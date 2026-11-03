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

public class ListarProblemasTicketsQuery : IRequest<ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
}

internal sealed class ListarProblemasTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarProblemasTicketsQuery, ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>> Handle(ListarProblemasTicketsQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x["IdProblema"]).ThenBy(x => x["IdTicket"]).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new ProblemasTicketsResponseDto()
            {
                IdProblema = (Guid)entity["IdProblema"],
                IdTicket = (Guid)entity["IdTicket"]
            })).ToListAsync();
            return new ResponseDto<PaginaDto<ProblemasTicketsResponseDto>>(200, new PaginaDto<ProblemasTicketsResponseDto>(items, request.Pagina, request.Tamano, total));
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
