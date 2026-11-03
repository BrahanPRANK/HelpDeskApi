using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ResumenTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ResumenTickets;

public class ListarResumenTicketsQuery : IRequest<ResponseDto<PaginaDto<ResumenTicketsResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Estado { get; set; }
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarResumenTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarResumenTicketsQuery, ResponseDto<PaginaDto<ResumenTicketsResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ResumenTicketsResponseDto>>> Handle(ListarResumenTicketsQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.ResumenTickets>().Query());
            if (!string.IsNullOrWhiteSpace(request.Estado)) query = query.Where(x => x.Estado == request.Estado);
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.Numero).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new ResumenTicketsResponseDto
            {
                IdTicket = entity.IdTicket,
                Numero = entity.Numero,
                Asunto = entity.Asunto,
                Tipo = entity.Tipo,
                Estado = entity.Estado,
                Prioridad = entity.Prioridad,
                IdSolicitante = entity.IdSolicitante,
                IdAreaSolicitante = entity.IdAreaSolicitante,
                IdEquipoAsignado = entity.IdEquipoAsignado,
                IdTecnicoAsignado = entity.IdTecnicoAsignado,
                FechaCreacion = entity.FechaCreacion,
                FechaLimiteRespuesta = entity.FechaLimiteRespuesta,
                FechaLimiteResolucion = entity.FechaLimiteResolucion,
                FechaPrimeraRespuesta = entity.FechaPrimeraRespuesta,
                FechaResolucion = entity.FechaResolucion,
                FechaCierre = entity.FechaCierre,
                MinutosHastaResolucion = entity.MinutosHastaResolucion,
                RespuestaEnPlazo = entity.RespuestaEnPlazo,
                ResolucionEnPlazo = entity.ResolucionEnPlazo,
                Calificacion = entity.Calificacion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<ResumenTicketsResponseDto>>(200, new PaginaDto<ResumenTicketsResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<ResumenTicketsResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<ResumenTicketsResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
