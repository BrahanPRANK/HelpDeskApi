using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Aprobaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones;

public class ListarAprobacionesQuery : IRequest<ResponseDto<PaginaDto<AprobacionesResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public string? Estado { get; set; }
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarAprobacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAprobacionesQuery, ResponseDto<PaginaDto<AprobacionesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AprobacionesResponseDto>>> Handle(ListarAprobacionesQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Motivo.Contains(request.Buscar));
            if (!string.IsNullOrWhiteSpace(request.Estado)) query = query.Where(x => x.Estado == request.Estado);
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAprobacion).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new AprobacionesResponseDto
            {
                IdAprobacion = entity.IdAprobacion,
                IdTicket = entity.IdTicket,
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdAprobador = entity.IdAprobador,
                Orden = entity.Orden,
                Motivo = entity.Motivo,
                Estado = entity.Estado,
                ComentarioDecision = entity.ComentarioDecision,
                FechaSolicitud = entity.FechaSolicitud,
                FechaDecision = entity.FechaDecision
            })).ToListAsync();
            return new ResponseDto<PaginaDto<AprobacionesResponseDto>>(200, new PaginaDto<AprobacionesResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<AprobacionesResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<AprobacionesResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
