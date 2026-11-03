using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
using HelpDesk.Application.Dtos.Tickets;
namespace HelpDesk.Application.UseCases.Tickets.Commands;

public record ResolverTicketCommand(Guid IdTicket, AccionTicketDto Datos) : IRequest<ResponseDto<TicketsResponseDto>>;

internal sealed class ResolverTicketCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access, ILogger<ResolverTicketCommandHandler> _logger)
    : IRequestHandler<ResolverTicketCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(ResolverTicketCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Check(request.Datos);
            var t = await Required<global::HelpDesk.Domain.Models.Tickets>(request.IdTicket);
            access.EnsureRead(t);
            Validation.Require(t.Version == request.Datos.Version, "El ticket cambio; consulte su version actual.", 409);
            var before = t.Estado;
            var now = DateTime.UtcNow;
            access.EnsureWork(t);
            Validation.Require(t.Estado == "en_atencion", "Debe estar en atencion.", 409);
            t.Estado = "resuelto";
            t.Solucion = request.Datos.Motivo;
            t.FechaResolucion = now;
            await History(t, "resolver", before, t.Estado, request.Datos.Motivo);
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
                _logger.LogError(ex, "Error controlado en ResolverTicketCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ResolverTicketCommandHandler.");
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
