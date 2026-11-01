using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Comentarios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Comentarios;

public record ListarComentariosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<ComentariosResponseDto>>>;

internal sealed class ListarComentariosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarComentariosQueryHandler> _logger) : IRequestHandler<ListarComentariosQuery, ResponseDto<PaginaDto<ComentariosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ComentariosResponseDto>>> Handle(ListarComentariosQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Comentarios>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Mensaje.Contains(options.Buscar));
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdComentario).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new ComentariosResponseDto { IdComentario = entity.IdComentario, IdTicket = entity.IdTicket, IdAutor = entity.IdAutor, Mensaje = entity.Mensaje, EsInterno = entity.EsInterno, Fecha = entity.Fecha }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarComentariosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarComentariosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
