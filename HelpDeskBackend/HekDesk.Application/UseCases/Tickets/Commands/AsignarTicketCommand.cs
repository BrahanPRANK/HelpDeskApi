using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
using HelpDesk.Domain.Dtos.Tickets;
namespace HelpDesk.Application.UseCases.Tickets.Commands;

public record AsignarTicketCommand(Guid IdTicket, AccionTicketDto Datos) : IRequest<ResponseDto<TicketsResponseDto>>;

internal sealed class AsignarTicketCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access, ILogger<AsignarTicketCommandHandler> _logger)
    : IRequestHandler<AsignarTicketCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(AsignarTicketCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Check(request.Datos);
            var t = await Required<global::HelpDesk.Domain.Models.Tickets>(request.IdTicket);
            access.EnsureRead(t);
            Validation.Require(t.Version == request.Datos.Version, "El ticket cambio; consulte su version actual.", 409);
            var before = t.Estado;
            var now = DateTime.UtcNow;
            access.Permission("asignaciones.administrar");
            Validation.Require(t.Estado == "nuevo", "Solo se asigna un ticket nuevo en esta primera version.", 409);
            var team = request.Datos.IdEquipo ?? t.IdEquipoAsignado;
            var tech = request.Datos.IdTecnico ?? throw new BusinessException(400, "Indique idTecnico.");
            if (!access.Admin)
                Validation.Require(_unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().Query().Any(m => m.IdEquipo == team && m.IdUsuario == actor.Id && m.Activo && m.EsCoordinador), "No coordina el equipo destino.", 403);
            Validation.Require(_unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>().Query().Any(e => e.IdEquipo == team && e.Activo), "Equipo inactivo.");
            Validation.Require(_unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().Query().Any(m => m.IdEquipo == team && m.IdUsuario == tech && m.Activo && m.Disponible), "Tecnico no disponible en el equipo.");
            Validation.Require((await Required<global::HelpDesk.Domain.Models.Usuarios>(tech)).Activo, "Tecnico inactivo.");
            var asignacion = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>()
            .Query().SingleOrDefaultAsync(x => x.IdTicket == t.IdTicket && x.FechaFin == null);
            if (asignacion is null)
            {
                asignacion = new global::HelpDesk.Domain.Models.AsignacionesTickets
                {
                    IdAsignacionTicket = Guid.NewGuid(),

                    IdTicket = t.IdTicket,

                    IdAsignador = actor.Id,

                    FechaAsignacion = now
                };
                await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().AddAsync(asignacion);
            }
            asignacion.IdAsignador = actor.Id;
            asignacion.FechaAsignacion = now;
            asignacion.IdEquipo = team;
            asignacion.IdTecnico = tech;
            asignacion.Motivo = request.Datos.Motivo;
            t.IdEquipoAsignado = team;
            t.IdTecnicoAsignado = tech;
            t.Estado = "asignado";
            await History(t, "asignar", before, t.Estado, request.Datos.Motivo);
            t.Version++;
            t.FechaActualizacion = now;
            await _unitOfWork.Complete();
            return new(200, new TicketsResponseDto
            {
                IdTicket = t.IdTicket,
                Numero = t.Numero,
                IdSolicitante = t.IdSolicitante,
                IdCreador = t.IdCreador,
                IdAreaSolicitante = t.IdAreaSolicitante,
                IdServicio = t.IdServicio,
                IdEquipoAsignado = t.IdEquipoAsignado,
                IdTecnicoAsignado = t.IdTecnicoAsignado,
                IdPolitica = t.IdPolitica,
                Tipo = t.Tipo,
                Asunto = t.Asunto,
                Descripcion = t.Descripcion,
                Prioridad = t.Prioridad,
                Estado = t.Estado,
                MotivoEspera = t.MotivoEspera,
                Solucion = t.Solucion,
                MotivoCancelacion = t.MotivoCancelacion,
                MinutosPrimeraRespuesta = t.MinutosPrimeraRespuesta,
                MinutosResolucion = t.MinutosResolucion,
                FechaCreacion = t.FechaCreacion,
                FechaActualizacion = t.FechaActualizacion,
                FechaLimiteRespuesta = t.FechaLimiteRespuesta,
                FechaLimiteResolucion = t.FechaLimiteResolucion,
                FechaPrimeraRespuesta = t.FechaPrimeraRespuesta,
                FechaResolucion = t.FechaResolucion,
                FechaCierre = t.FechaCierre,
                FechaCancelacion = t.FechaCancelacion,
                Version = t.Version
            }, "Accion completada.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en AsignarTicketCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar AsignarTicketCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
    private async Task History(global::HelpDesk.Domain.Models.Tickets ticket, string action, string? before, string? after, string reason) =>
        await _unitOfWork.Repository<global::HelpDesk.Domain.Models.HistorialTickets>().AddAsync(new global::HelpDesk.Domain.Models.HistorialTickets
        {
            IdHistorial = Guid.NewGuid(),

            IdTicket = ticket.IdTicket,

            IdUsuario = actor.Id,

            Accion = action,

            CampoModificado = "estado",

            ValorAnterior = before,

            ValorNuevo = after,

            Motivo = reason,

            Fecha = DateTime.UtcNow
        });
}
