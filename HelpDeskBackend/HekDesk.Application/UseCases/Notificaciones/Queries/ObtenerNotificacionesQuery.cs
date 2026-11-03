using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Notificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones;

public record ObtenerNotificacionesQuery(object[] Keys) : IRequest<ResponseDto<NotificacionesResponseDto>>;

internal sealed class ObtenerNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerNotificacionesQuery, ResponseDto<NotificacionesResponseDto>>
{
    public async Task<ResponseDto<NotificacionesResponseDto>> Handle(ObtenerNotificacionesQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<NotificacionesResponseDto>(200, new NotificacionesResponseDto
            {
                IdNotificacion = entity.IdNotificacion,
                IdUsuario = entity.IdUsuario,
                IdTicket = entity.IdTicket,
                Titulo = entity.Titulo,
                Mensaje = entity.Mensaje,
                FechaCreacion = entity.FechaCreacion,
                FechaLectura = entity.FechaLectura,
                ClaveEvento = entity.ClaveEvento,
                TipoEvento = entity.TipoEvento
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<NotificacionesResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<NotificacionesResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
