using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Aprobaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones;

public class CrearAprobacionesCommand : IRequest<ResponseDto<AprobacionesResponseDto>>
{
    public Guid? IdTicket { get; set; }
    public Guid? IdCambio { get; set; }
    public Guid IdAprobador { get; set; }
    public short Orden { get; set; } = 1;
    [Required]
    [StringLength(10000)]
    public string Motivo { get; set; } = "";
    public string? ComentarioDecision { get; set; }
}

internal sealed class CrearAprobacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearAprobacionesCommandHandler> _logger) : IRequestHandler<CrearAprobacionesCommand, ResponseDto<AprobacionesResponseDto>>
{
    public async Task<ResponseDto<AprobacionesResponseDto>> Handle(CrearAprobacionesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Aprobaciones()
            {
                IdAprobacion = Guid.NewGuid(),
                IdTicket = request.IdTicket,
                IdCambio = request.IdCambio,
                IdAprobador = request.IdAprobador,
                Orden = request.Orden,
                Motivo = request.Motivo,
                ComentarioDecision = request.ComentarioDecision,
                FechaSolicitud = DateTime.UtcNow,
                Estado = "pendiente"
            };
            if (true) entity.IdSolicitante = actor.Id;
            Validation.Require(entity.IdAprobador != entity.IdSolicitante, "No se permite autoaprobacion.");
            var approver = await Required<global::HelpDesk.Domain.Models.Usuarios>(entity.IdAprobador);
            var approverRoles = _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Where(x => x.IdUsuario == approver.IdUsuario).Select(x => x.IdRol);
            Validation.Require(approver.Activo && _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query().Any(x => approverRoles.Contains(x.IdRol) && x.Nombre == "aprobador"), "El usuario debe tener rol aprobador y estar activo.");
            if (entity.IdTicket is Guid approvalTicket) access.EnsureRead(await Required<global::HelpDesk.Domain.Models.Tickets>(approvalTicket));
            if (entity.IdCambio is Guid approvalChange) access.EnsureRead(await Required<global::HelpDesk.Domain.Models.Cambios>(approvalChange));
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new AprobacionesResponseDto
            {
                IdAprobacion = entity.IdAprobacion,
                IdTicket = entity.IdTicket,
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdAprobador = entity.IdAprobador,
                Orden = entity.Orden,
                Motivo = entity.Motivo,
                Estado = entity.Estado,
                ComentarioDecision = entity.ComentarioDecision,
                FechaSolicitud = entity.FechaSolicitud,
                FechaDecision = entity.FechaDecision
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearAprobacionesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearAprobacionesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
