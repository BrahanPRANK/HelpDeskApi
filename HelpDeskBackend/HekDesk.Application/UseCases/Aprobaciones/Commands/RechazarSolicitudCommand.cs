using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones.Commands;

public record RechazarSolicitudCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class RechazarSolicitudCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access, ILogger<RechazarSolicitudCommandHandler> _logger)
    : IRequestHandler<RechazarSolicitudCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(RechazarSolicitudCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("aprobaciones.decidir");
            var a=await Required<global::HelpDesk.Domain.Models.Aprobaciones>(r.Id,ct);
            Validation.Require(a.IdAprobador==actor.Id,"Solo el aprobador asignado puede decidir.",403);
            Validation.Require(a.Estado=="pendiente","Ya fue decidida.",409);
            var previous=await _unitOfWork.ListAsync(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().Query().Where(x=>x.IdTicket==a.IdTicket && x.IdCambio==a.IdCambio && x.Orden<a.Orden),ct);
            Validation.Require(previous.All(x=>x.Estado=="aprobado"),"Complete los pasos anteriores en orden.",409);
            Validation.Require(!string.IsNullOrWhiteSpace(r.Datos.Motivo),"Indique el motivo de la decision.");
            a.Estado="rechazado";a.FechaDecision=DateTime.UtcNow;a.ComentarioDecision=r.Datos.Motivo;result=a.Estado;
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en RechazarSolicitudCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar RechazarSolicitudCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
