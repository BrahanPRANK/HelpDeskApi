using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones.Commands;

public record MarcarNotificacionLeidaCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class MarcarNotificacionLeidaCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, ILogger<MarcarNotificacionLeidaCommandHandler> _logger)
    : IRequestHandler<MarcarNotificacionLeidaCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(MarcarNotificacionLeidaCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            var notification=await Required<global::HelpDesk.Domain.Models.Notificaciones>(r.Id,ct);
            Validation.Require(notification.IdUsuario==actor.Id,"Solo puede marcar sus propios avisos.",403);
            notification.FechaLectura ??=DateTime.UtcNow;result="leida";
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en MarcarNotificacionLeidaCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar MarcarNotificacionLeidaCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
