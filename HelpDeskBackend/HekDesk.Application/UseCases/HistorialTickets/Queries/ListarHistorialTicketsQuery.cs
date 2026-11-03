using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.HistorialTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.HistorialTickets;

public class ListarHistorialTicketsQuery : IRequest<ResponseDto<PaginaDto<HistorialTicketsResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarHistorialTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarHistorialTicketsQuery, ResponseDto<PaginaDto<HistorialTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<HistorialTicketsResponseDto>>> Handle(ListarHistorialTicketsQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.HistorialTickets>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Accion.Contains(request.Buscar));
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdHistorial).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new HistorialTicketsResponseDto
            {
                IdHistorial = entity.IdHistorial,
                IdTicket = entity.IdTicket,
                IdUsuario = entity.IdUsuario,
                Accion = entity.Accion,
                CampoModificado = entity.CampoModificado,
                ValorAnterior = entity.ValorAnterior,
                ValorNuevo = entity.ValorNuevo,
                Motivo = entity.Motivo,
                Fecha = entity.Fecha
            })).ToListAsync();
            return new ResponseDto<PaginaDto<HistorialTicketsResponseDto>>(200, new PaginaDto<HistorialTicketsResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<HistorialTicketsResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<HistorialTicketsResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
