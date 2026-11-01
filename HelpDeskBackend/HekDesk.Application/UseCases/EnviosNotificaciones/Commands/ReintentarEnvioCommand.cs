using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EnviosNotificaciones.Commands;

public record ReintentarEnvioCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class ReintentarEnvioCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ReintentarEnvioCommandHandler> _logger)
    : IRequestHandler<ReintentarEnvioCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(ReintentarEnvioCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("usuarios.administrar");
            var delivery=await Required<global::HelpDesk.Domain.Models.EnviosNotificaciones>(r.Id,ct);
            Validation.Require(delivery.Estado=="fallido" && delivery.Intentos<delivery.MaximoIntentos,"No puede reintentarse este envio.",409);
            delivery.Estado="pendiente";delivery.FechaProximoIntento=DateTime.UtcNow;delivery.BloqueoHasta=null;result="pendiente";
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ReintentarEnvioCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ReintentarEnvioCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
