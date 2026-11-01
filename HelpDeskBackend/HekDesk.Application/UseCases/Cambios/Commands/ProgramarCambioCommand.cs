using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios.Commands;

public record ProgramarCambioCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class ProgramarCambioCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ProgramarCambioCommandHandler> _logger)
    : IRequestHandler<ProgramarCambioCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(ProgramarCambioCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("cambios.gestionar");
            var change=await Required<global::HelpDesk.Domain.Models.Cambios>(r.Id,ct);access.EnsureRead(change);
            Validation.Require(change.Estado is "borrador" or "pendiente_aprobacion" or "aprobado","Estado no permitido.",409);
            var approvals=await _unitOfWork.ListAsync(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().Query().Where(x=>x.IdCambio==change.IdCambio),ct);
            Validation.Require((!change.RequiereAprobacion || approvals.Count>0) && approvals.All(x=>x.Estado=="aprobado"),"Faltan aprobaciones.",409);
            Validation.Require(r.Datos.FechaProgramada.HasValue && r.Datos.FechaProgramada>=DateTime.UtcNow,"Indique una fecha futura.");
            change.FechaProgramada=r.Datos.FechaProgramada;change.Estado="programado";

            result=change.Estado;
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ProgramarCambioCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ProgramarCambioCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
