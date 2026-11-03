using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PreferenciasNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones;

public class ActualizarPreferenciasNotificacionesCommand : IRequest<ResponseDto<PreferenciasNotificacionesResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdUsuario { get; set; }
    [Required]
    [StringLength(10000)]
    public string TipoEvento { get; set; } = "";
    public bool RecibirEnSistema { get; set; }
    public bool RecibirPorCorreo { get; set; }
}

internal sealed class ActualizarPreferenciasNotificacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<ActualizarPreferenciasNotificacionesCommandHandler> _logger) : IRequestHandler<ActualizarPreferenciasNotificacionesCommand, ResponseDto<PreferenciasNotificacionesResponseDto>>
{
    public async Task<ResponseDto<PreferenciasNotificacionesResponseDto>> Handle(ActualizarPreferenciasNotificacionesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            if (!access.Admin) access.Permission("notificaciones.propias");
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.RecibirEnSistema = request.RecibirEnSistema;
            entity.RecibirPorCorreo = request.RecibirPorCorreo;
            Validation.Require(access.Admin || entity.IdUsuario == actor.Id, "Solo puede modificar preferencias propias.", 403);
            await _unitOfWork.Complete();
            return new(200, new PreferenciasNotificacionesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                TipoEvento = entity.TipoEvento,
                RecibirEnSistema = entity.RecibirEnSistema,
                RecibirPorCorreo = entity.RecibirPorCorreo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarPreferenciasNotificacionesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarPreferenciasNotificacionesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
