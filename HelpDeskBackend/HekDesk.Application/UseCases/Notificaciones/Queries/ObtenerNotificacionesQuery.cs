using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Notificaciones.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones;

public record ObtenerNotificacionesQuery(object[] Keys) : IRequest<ResponseDto<NotificacionesResponseDto>>;

internal sealed class ObtenerNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerNotificacionesQueryHandler> _logger) : IRequestHandler<ObtenerNotificacionesQuery, ResponseDto<NotificacionesResponseDto>>
{
    public async Task<ResponseDto<NotificacionesResponseDto>> Handle(ObtenerNotificacionesQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new NotificacionesResponseDto { IdNotificacion = entity.IdNotificacion, IdUsuario = entity.IdUsuario, IdTicket = entity.IdTicket, Titulo = entity.Titulo, Mensaje = entity.Mensaje, FechaCreacion = entity.FechaCreacion, FechaLectura = entity.FechaLectura, ClaveEvento = entity.ClaveEvento, TipoEvento = entity.TipoEvento });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerNotificacionesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerNotificacionesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
