using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Adjuntos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Adjuntos;

public record ObtenerAdjuntosQuery(object[] Keys) : IRequest<ResponseDto<AdjuntosResponseDto>>;

internal sealed class ObtenerAdjuntosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerAdjuntosQueryHandler> _logger) : IRequestHandler<ObtenerAdjuntosQuery, ResponseDto<AdjuntosResponseDto>>
{
    public async Task<ResponseDto<AdjuntosResponseDto>> Handle(ObtenerAdjuntosQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Adjuntos>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new AdjuntosResponseDto { IdAdjunto = entity.IdAdjunto, IdTicket = entity.IdTicket, IdComentario = entity.IdComentario, IdUsuario = entity.IdUsuario, NombreArchivo = entity.NombreArchivo, TipoArchivo = entity.TipoArchivo, TamanoBytes = entity.TamanoBytes, EsInterno = entity.EsInterno, Fecha = entity.Fecha });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerAdjuntosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerAdjuntosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
