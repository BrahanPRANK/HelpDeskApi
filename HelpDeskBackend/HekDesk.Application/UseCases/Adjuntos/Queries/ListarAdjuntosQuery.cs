using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Adjuntos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Adjuntos;

public record ListarAdjuntosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<AdjuntosResponseDto>>>;

internal sealed class ListarAdjuntosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarAdjuntosQueryHandler> _logger) : IRequestHandler<ListarAdjuntosQuery, ResponseDto<PaginaDto<AdjuntosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AdjuntosResponseDto>>> Handle(ListarAdjuntosQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Adjuntos>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.NombreArchivo.Contains(options.Buscar) || x.TipoArchivo.Contains(options.Buscar));
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdAdjunto).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new AdjuntosResponseDto { IdAdjunto = entity.IdAdjunto, IdTicket = entity.IdTicket, IdComentario = entity.IdComentario, IdUsuario = entity.IdUsuario, NombreArchivo = entity.NombreArchivo, TipoArchivo = entity.TipoArchivo, TamanoBytes = entity.TamanoBytes, EsInterno = entity.EsInterno, Fecha = entity.Fecha }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarAdjuntosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarAdjuntosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
