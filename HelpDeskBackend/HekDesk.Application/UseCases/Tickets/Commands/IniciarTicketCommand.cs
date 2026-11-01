using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
using HelpDesk.Application.UseCases.Tickets.Dtos;
namespace HelpDesk.Application.UseCases.Tickets.Commands;

public record IniciarTicketCommand(Guid IdTicket, AccionTicketDto Datos) : IRequest<ResponseDto<TicketsResponseDto>>;

internal sealed class IniciarTicketCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access, ILogger<IniciarTicketCommandHandler> _logger)
    : IRequestHandler<IniciarTicketCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(IniciarTicketCommand request, CancellationToken ct) {
        try
        {
            return await _unitOfWork.InTransactionAsync(() => Ejecutar(request, ct), ct);
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en IniciarTicketCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar IniciarTicketCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<ResponseDto<TicketsResponseDto>> Ejecutar(IniciarTicketCommand request, CancellationToken ct)
    {
        Validation.Check(request.Datos);
        var t=await Required<global::HelpDesk.Domain.Models.Tickets>(request.IdTicket,ct);access.EnsureRead(t);
        Validation.Require(t.Version==request.Datos.Version,"El ticket cambio; consulte su version actual.",409);
        var before=t.Estado;var now=DateTime.UtcNow;
        var active=t.Estado is not ("resuelto" or "cerrado" or "cancelado");
                access.EnsureWork(t);Validation.Require(t.Estado=="asignado","Debe estar asignado.",409);
                await EnsureApprovals(t,ct);t.Estado="en_atencion";
        History(t,"iniciar",before,t.Estado,request.Datos.Motivo);
        _unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().Add(new global::HelpDesk.Domain.Models.Notificaciones {IdNotificacion=Guid.NewGuid(),IdUsuario=t.IdSolicitante,IdTicket=t.IdTicket,
            Titulo="Actualizacion de ticket",Mensaje=$"Ticket {t.Numero}: {t.Estado}",TipoEvento="ticket_actualizado",
            ClaveEvento=Guid.NewGuid().ToString("N"),FechaCreacion=now});
        await _unitOfWork.Complete(ct);
        return new(200,new TicketsResponseDto { IdTicket = t.IdTicket, Numero = t.Numero, IdSolicitante = t.IdSolicitante, IdCreador = t.IdCreador, IdAreaSolicitante = t.IdAreaSolicitante, IdServicio = t.IdServicio, IdEquipoAsignado = t.IdEquipoAsignado, IdTecnicoAsignado = t.IdTecnicoAsignado, IdPolitica = t.IdPolitica, Tipo = t.Tipo, Asunto = t.Asunto, Descripcion = t.Descripcion, Prioridad = t.Prioridad, Estado = t.Estado, MotivoEspera = t.MotivoEspera, Solucion = t.Solucion, MotivoCancelacion = t.MotivoCancelacion, MinutosPrimeraRespuesta = t.MinutosPrimeraRespuesta, MinutosResolucion = t.MinutosResolucion, FechaCreacion = t.FechaCreacion, FechaActualizacion = t.FechaActualizacion, FechaLimiteRespuesta = t.FechaLimiteRespuesta, FechaLimiteResolucion = t.FechaLimiteResolucion, FechaPrimeraRespuesta = t.FechaPrimeraRespuesta, FechaResolucion = t.FechaResolucion, FechaCierre = t.FechaCierre, FechaCancelacion = t.FechaCancelacion, Version = t.Version },"Accion completada.");

    }
    private async Task EnsureApprovals(global::HelpDesk.Domain.Models.Tickets ticket,CancellationToken ct)
    {
        var service=await Required<global::HelpDesk.Domain.Models.Servicios>(ticket.IdServicio,ct);
        var approvals=await _unitOfWork.ListAsync(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().Query().Where(a=>a.IdTicket==ticket.IdTicket),ct);
        Validation.Require((!service.RequiereAprobacion || approvals.Count>0) && approvals.All(x=>x.Estado=="aprobado"),
            "Complete las aprobaciones requeridas antes de continuar.",409);
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
    private void History(global::HelpDesk.Domain.Models.Tickets ticket,string action,string? before,string? after,string reason) =>
        _unitOfWork.Repository<global::HelpDesk.Domain.Models.HistorialTickets>().Add(new global::HelpDesk.Domain.Models.HistorialTickets {IdHistorial=Guid.NewGuid(),IdTicket=ticket.IdTicket,IdUsuario=actor.Id,Accion=action,
            CampoModificado="estado",ValorAnterior=before,ValorNuevo=after,Motivo=reason,Fecha=DateTime.UtcNow});}
