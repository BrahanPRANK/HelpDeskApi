using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Notificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones;

public class ActualizarNotificacionesCommand : IRequest<ResponseDto<NotificacionesResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdUsuario { get; set; }
    public Guid? IdTicket { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Mensaje { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string ClaveEvento { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TipoEvento { get; set; } = "";
}

internal sealed class ActualizarNotificacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ActualizarNotificacionesCommand, ResponseDto<NotificacionesResponseDto>>
{
    public async Task<ResponseDto<NotificacionesResponseDto>> Handle(ActualizarNotificacionesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdUsuario = request.IdUsuario;
            entity.IdTicket = request.IdTicket;
            entity.Titulo = request.Titulo;
            entity.Mensaje = request.Mensaje;
            entity.ClaveEvento = request.ClaveEvento;
            entity.TipoEvento = request.TipoEvento;

            await _unitOfWork.Complete();
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
            }, "Registro actualizado.");
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
