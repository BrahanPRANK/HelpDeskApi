using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
using HelpDesk.Domain.Dtos.Tickets;
namespace HelpDesk.Application.UseCases.Tickets.Commands;

public class CerrarTicketCommand : IRequest<ResponseDto<TicketsResponseDto>>
{
    [JsonIgnore]
    public Guid IdTicket { get; set; }

    [Range(1, int.MaxValue)]
    public int Version { get; set; }
    [Required, StringLength(4000)]
    public string Motivo { get; set; } = string.Empty;
    public Guid? IdEquipo { get; set; }
    public Guid? IdTecnico { get; set; }
}

internal sealed class CerrarTicketCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access)
    : IRequestHandler<CerrarTicketCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(CerrarTicketCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Check(request);
            var t = await Required<global::HelpDesk.Domain.Models.Tickets>(request.IdTicket);
            access.EnsureRead(t);
            Validation.Require(t.Version == request.Version, "El ticket cambio; consulte su version actual.", 409);
            var before = t.Estado;
            var now = DateTime.UtcNow;
            Validation.Require(t.IdSolicitante == actor.Id, "Solo el solicitante confirma el cierre.", 403);
            access.Permission("tickets.confirmar_propios");
            Validation.Require(t.Estado == "resuelto", "Debe estar resuelto.", 409);
            t.Estado = "cerrado";
            t.FechaCierre = now;
            var asignacion = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>()
                .Query().SingleOrDefaultAsync(x => x.IdTicket == t.IdTicket && x.FechaFin == null);
            if (asignacion is not null) asignacion.FechaFin = now;
            await History(t, "cerrar", before, t.Estado, request.Motivo);
            t.Version++;
            t.FechaActualizacion = now;
            await _unitOfWork.Complete();
            return new ResponseDto<TicketsResponseDto>(200, new TicketsResponseDto
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
            return new ResponseDto<TicketsResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<TicketsResponseDto>(500, default, "No se pudo completar la operacion.");
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
