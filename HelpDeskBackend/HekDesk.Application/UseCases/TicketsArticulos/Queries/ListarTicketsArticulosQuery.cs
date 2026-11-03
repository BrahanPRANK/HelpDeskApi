using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TicketsArticulos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsArticulos;

public class ListarTicketsArticulosQuery : IRequest<ResponseDto<PaginaDto<TicketsArticulosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarTicketsArticulosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarTicketsArticulosQuery, ResponseDto<PaginaDto<TicketsArticulosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<TicketsArticulosResponseDto>>> Handle(ListarTicketsArticulosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.TicketsArticulos>().Query());
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdTicket).ThenBy(x => x.IdArticulo).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new TicketsArticulosResponseDto
            {
                IdTicket = entity.IdTicket,
                IdArticulo = entity.IdArticulo,
                IdUsuario = entity.IdUsuario,
                FechaVinculacion = entity.FechaVinculacion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<TicketsArticulosResponseDto>>(200, new PaginaDto<TicketsArticulosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<TicketsArticulosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<TicketsArticulosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
