using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Adjuntos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Adjuntos;

public class ListarAdjuntosQuery : IRequest<ResponseDto<PaginaDto<AdjuntosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarAdjuntosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAdjuntosQuery, ResponseDto<PaginaDto<AdjuntosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AdjuntosResponseDto>>> Handle(ListarAdjuntosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Adjuntos>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.NombreArchivo.Contains(request.Buscar) || x.TipoArchivo.Contains(request.Buscar));
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAdjunto).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new AdjuntosResponseDto
            {
                IdAdjunto = entity.IdAdjunto,
                IdTicket = entity.IdTicket,
                IdComentario = entity.IdComentario,
                IdUsuario = entity.IdUsuario,
                NombreArchivo = entity.NombreArchivo,
                TipoArchivo = entity.TipoArchivo,
                TamanoBytes = entity.TamanoBytes,
                EsInterno = entity.EsInterno,
                Fecha = entity.Fecha
            })).ToListAsync();
            return new ResponseDto<PaginaDto<AdjuntosResponseDto>>(200, new PaginaDto<AdjuntosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<AdjuntosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<AdjuntosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
