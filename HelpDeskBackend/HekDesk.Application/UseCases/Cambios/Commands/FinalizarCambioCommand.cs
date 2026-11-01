using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios.Commands;

public record FinalizarCambioCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class FinalizarCambioCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<FinalizarCambioCommandHandler> _logger)
    : IRequestHandler<FinalizarCambioCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(FinalizarCambioCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("cambios.gestionar");
            var change=await Required<global::HelpDesk.Domain.Models.Cambios>(r.Id,ct);access.EnsureRead(change);
            Validation.Require(change.Estado=="en_ejecucion","Debe estar en ejecucion.",409);
            Validation.Require(!string.IsNullOrWhiteSpace(r.Datos.Motivo),"Indique resultado.");
            Validation.Require(!_unitOfWork.Repository<global::HelpDesk.Domain.Models.TareasCambios>().Query().Any(x=>x.IdCambio==change.IdCambio && x.Estado!="completada" && x.Estado!="cancelada"),"Hay tareas pendientes.",409);
            change.Estado="finalizado";change.Resultado=r.Datos.Motivo;change.FechaFinalizacion=DateTime.UtcNow;

            result=change.Estado;
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en FinalizarCambioCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar FinalizarCambioCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
