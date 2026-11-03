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

public record ListarAdjuntosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<AdjuntosResponseDto>>>;

internal sealed class ListarAdjuntosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAdjuntosQuery, ResponseDto<PaginaDto<AdjuntosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AdjuntosResponseDto>>> Handle(ListarAdjuntosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Adjuntos>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.NombreArchivo.Contains(options.Buscar) || x.TipoArchivo.Contains(options.Buscar));
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAdjunto).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
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
            return new ResponseDto<PaginaDto<AdjuntosResponseDto>>(200, new PaginaDto<AdjuntosResponseDto>(items, options.Pagina, options.Tamano, total));
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
