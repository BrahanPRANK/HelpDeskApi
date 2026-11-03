using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PreferenciasNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones;

public class CrearPreferenciasNotificacionesCommand : IRequest<ResponseDto<PreferenciasNotificacionesResponseDto>>
{
    public Guid IdUsuario { get; set; }
    [Required]
    [StringLength(10000)]
    public string TipoEvento { get; set; } = "";
    public bool RecibirEnSistema { get; set; }
    public bool RecibirPorCorreo { get; set; }
}

internal sealed class CrearPreferenciasNotificacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor) : IRequestHandler<CrearPreferenciasNotificacionesCommand, ResponseDto<PreferenciasNotificacionesResponseDto>>
{
    public async Task<ResponseDto<PreferenciasNotificacionesResponseDto>> Handle(CrearPreferenciasNotificacionesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            if (!access.Admin) access.Permission("notificaciones.propias");
            var entity = new global::HelpDesk.Domain.Models.PreferenciasNotificaciones()
            {
                IdUsuario = request.IdUsuario,
                TipoEvento = request.TipoEvento,
                RecibirEnSistema = request.RecibirEnSistema,
                RecibirPorCorreo = request.RecibirPorCorreo
            };
            Validation.Require(access.Admin || entity.IdUsuario == actor.Id, "Solo puede modificar preferencias propias.", 403);
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<PreferenciasNotificacionesResponseDto>(201, new PreferenciasNotificacionesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                TipoEvento = entity.TipoEvento,
                RecibirEnSistema = entity.RecibirEnSistema,
                RecibirPorCorreo = entity.RecibirPorCorreo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PreferenciasNotificacionesResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PreferenciasNotificacionesResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
