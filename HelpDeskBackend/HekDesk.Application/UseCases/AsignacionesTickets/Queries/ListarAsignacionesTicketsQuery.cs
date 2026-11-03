using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.AsignacionesTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesTickets;

public class ListarAsignacionesTicketsQuery : IRequest<ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarAsignacionesTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAsignacionesTicketsQuery, ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>> Handle(ListarAsignacionesTicketsQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Motivo.Contains(request.Buscar));
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAsignacionTicket).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
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
            return new ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>(200, new PaginaDto<AsignacionesTicketsResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<AsignacionesTicketsResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
