using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Aprobaciones.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones;

public record ActualizarAprobacionesCommand(object[] Keys, AprobacionesInputDto Datos) : IRequest<ResponseDto<AprobacionesResponseDto>>;

internal sealed class ActualizarAprobacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarAprobacionesCommandHandler> _logger) : IRequestHandler<ActualizarAprobacionesCommand, ResponseDto<AprobacionesResponseDto>>
{
    public async Task<ResponseDto<AprobacionesResponseDto>> Handle(ActualizarAprobacionesCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(entity.Estado=="pendiente","Una aprobacion decidida no se modifica.",409);
            entity.IdTicket = input.IdTicket;
            entity.IdCambio = input.IdCambio;
            entity.IdAprobador = input.IdAprobador;
            entity.Orden = input.Orden;
            entity.Motivo = input.Motivo;
            entity.ComentarioDecision = input.ComentarioDecision;

            Validation.Require(entity.IdAprobador!=entity.IdSolicitante,"No se permite autoaprobacion.");
            var approver=await Required<global::HelpDesk.Domain.Models.Usuarios>(entity.IdAprobador,ct);
            var approverRoles=_unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Where(x=>x.IdUsuario==approver.IdUsuario).Select(x=>x.IdRol);
            Validation.Require(approver.Activo && _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query().Any(x=>approverRoles.Contains(x.IdRol) && x.Nombre=="aprobador"),"El usuario debe tener rol aprobador y estar activo.");
            if(entity.IdTicket is Guid approvalTicket)access.EnsureRead(await Required<global::HelpDesk.Domain.Models.Tickets>(approvalTicket,ct));
            if(entity.IdCambio is Guid approvalChange)access.EnsureRead(await Required<global::HelpDesk.Domain.Models.Cambios>(approvalChange,ct));
            await _unitOfWork.Complete(ct);
            return new(200,new AprobacionesResponseDto { IdAprobacion = entity.IdAprobacion, IdTicket = entity.IdTicket, IdCambio = entity.IdCambio, IdSolicitante = entity.IdSolicitante, IdAprobador = entity.IdAprobador, Orden = entity.Orden, Motivo = entity.Motivo, Estado = entity.Estado, ComentarioDecision = entity.ComentarioDecision, FechaSolicitud = entity.FechaSolicitud, FechaDecision = entity.FechaDecision },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarAprobacionesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarAprobacionesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
