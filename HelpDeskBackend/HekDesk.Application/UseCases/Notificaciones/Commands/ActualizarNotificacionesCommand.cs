using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Notificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones;

public record ActualizarNotificacionesCommand(object[] Keys, NotificacionesInputDto Datos) : IRequest<ResponseDto<NotificacionesResponseDto>>;

internal sealed class ActualizarNotificacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarNotificacionesCommandHandler> _logger) : IRequestHandler<ActualizarNotificacionesCommand, ResponseDto<NotificacionesResponseDto>>
{
    public async Task<ResponseDto<NotificacionesResponseDto>> Handle(ActualizarNotificacionesCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdUsuario = input.IdUsuario;
            entity.IdTicket = input.IdTicket;
            entity.Titulo = input.Titulo;
            entity.Mensaje = input.Mensaje;
            entity.ClaveEvento = input.ClaveEvento;
            entity.TipoEvento = input.TipoEvento;

            await _unitOfWork.Complete();
            return new(200, new NotificacionesResponseDto
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
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarNotificacionesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarNotificacionesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
