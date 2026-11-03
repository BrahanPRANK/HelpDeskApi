using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.PreferenciasNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones;

public record CrearPreferenciasNotificacionesCommand(PreferenciasNotificacionesInputDto Datos) : IRequest<ResponseDto<PreferenciasNotificacionesResponseDto>>;

internal sealed class CrearPreferenciasNotificacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearPreferenciasNotificacionesCommandHandler> _logger) : IRequestHandler<CrearPreferenciasNotificacionesCommand, ResponseDto<PreferenciasNotificacionesResponseDto>>
{
    public async Task<ResponseDto<PreferenciasNotificacionesResponseDto>> Handle(CrearPreferenciasNotificacionesCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            if (!access.Admin) access.Permission("notificaciones.propias");
            var entity = new global::HelpDesk.Domain.Models.PreferenciasNotificaciones()
            {
                IdUsuario = input.IdUsuario,
                TipoEvento = input.TipoEvento,
                RecibirEnSistema = input.RecibirEnSistema,
                RecibirPorCorreo = input.RecibirPorCorreo
            };
            Validation.Require(access.Admin || entity.IdUsuario == actor.Id, "Solo puede modificar preferencias propias.", 403);
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new PreferenciasNotificacionesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                TipoEvento = entity.TipoEvento,
                RecibirEnSistema = entity.RecibirEnSistema,
                RecibirPorCorreo = entity.RecibirPorCorreo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearPreferenciasNotificacionesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearPreferenciasNotificacionesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
