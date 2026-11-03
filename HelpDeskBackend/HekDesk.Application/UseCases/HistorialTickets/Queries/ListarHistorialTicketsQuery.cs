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

public record ListarHistorialTicketsQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<HistorialTicketsResponseDto>>>;

internal sealed class ListarHistorialTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarHistorialTicketsQuery, ResponseDto<PaginaDto<HistorialTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<HistorialTicketsResponseDto>>> Handle(ListarHistorialTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.HistorialTickets>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Accion.Contains(options.Buscar));
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdHistorial).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
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
            return new ResponseDto<PaginaDto<HistorialTicketsResponseDto>>(200, new PaginaDto<HistorialTicketsResponseDto>(items, options.Pagina, options.Tamano, total));
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
