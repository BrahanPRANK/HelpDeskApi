using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones.Commands;

public class MarcarNotificacionLeidaCommand : IRequest<ResponseDto<AccionResultadoDto>>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [StringLength(4000)]
    public string Motivo { get; set; } = string.Empty;
}

internal sealed class MarcarNotificacionLeidaCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor)
    : IRequestHandler<MarcarNotificacionLeidaCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(MarcarNotificacionLeidaCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r);
            var result = "completado";
            var notification = await Required<global::HelpDesk.Domain.Models.Notificaciones>(r.Id);
            Validation.Require(notification.IdUsuario == actor.Id, "Solo puede marcar sus propios avisos.", 403);
            notification.FechaLectura ??= DateTime.UtcNow;
            result = "leida";
            await _unitOfWork.Complete();
            return new ResponseDto<AccionResultadoDto>(200, new AccionResultadoDto(r.Id, result));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AccionResultadoDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AccionResultadoDto>(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
