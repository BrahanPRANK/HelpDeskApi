using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Notificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones;

public record CrearNotificacionesCommand(NotificacionesInputDto Datos) : IRequest<ResponseDto<NotificacionesResponseDto>>;

internal sealed class CrearNotificacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearNotificacionesCommandHandler> _logger) : IRequestHandler<CrearNotificacionesCommand, ResponseDto<NotificacionesResponseDto>>
{
    public async Task<ResponseDto<NotificacionesResponseDto>> Handle(CrearNotificacionesCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Notificaciones()
            {
                IdNotificacion = Guid.NewGuid(),
                IdUsuario = input.IdUsuario,
                IdTicket = input.IdTicket,
                Titulo = input.Titulo,
                Mensaje = input.Mensaje,
                ClaveEvento = input.ClaveEvento,
                TipoEvento = input.TipoEvento,
                FechaCreacion = DateTime.UtcNow
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new NotificacionesResponseDto
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
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearNotificacionesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearNotificacionesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
