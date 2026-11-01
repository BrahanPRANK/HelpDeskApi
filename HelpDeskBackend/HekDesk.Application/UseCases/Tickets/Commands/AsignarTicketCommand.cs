using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
using HelpDesk.Application.UseCases.Tickets.Dtos;
namespace HelpDesk.Application.UseCases.Tickets.Commands;

public record AsignarTicketCommand(Guid IdTicket, AccionTicketDto Datos) : IRequest<ResponseDto<TicketsResponseDto>>;

internal sealed class AsignarTicketCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access, ILogger<AsignarTicketCommandHandler> _logger)
    : IRequestHandler<AsignarTicketCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(AsignarTicketCommand request, CancellationToken ct) {
        try
        {
            return await _unitOfWork.InTransactionAsync(() => Ejecutar(request, ct), ct);
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en AsignarTicketCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar AsignarTicketCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<ResponseDto<TicketsResponseDto>> Ejecutar(AsignarTicketCommand request, CancellationToken ct)
    {
        Validation.Check(request.Datos);
        var t=await Required<global::HelpDesk.Domain.Models.Tickets>(request.IdTicket,ct);access.EnsureRead(t);
        Validation.Require(t.Version==request.Datos.Version,"El ticket cambio; consulte su version actual.",409);
        var before=t.Estado;var now=DateTime.UtcNow;
        var active=t.Estado is not ("resuelto" or "cerrado" or "cancelado");
                access.Permission("asignaciones.administrar");Validation.Require(active,"El ticket ya fue finalizado.",409);
                var team=request.Datos.IdEquipo ?? t.IdEquipoAsignado;
                var tech=request.Datos.IdTecnico ?? throw new BusinessException(400,"Indique idTecnico.");
                if(!access.Admin)
                    Validation.Require(_unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().Query().Any(m=>m.IdEquipo==team && m.IdUsuario==actor.Id && m.Activo && m.EsCoordinador),"No coordina el equipo destino.",403);
                Validation.Require(_unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>().Query().Any(e=>e.IdEquipo==team && e.Activo),"Equipo inactivo.");
                Validation.Require(_unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().Query().Any(m=>m.IdEquipo==team && m.IdUsuario==tech && m.Activo && m.Disponible),"Tecnico no disponible en el equipo.");
                Validation.Require((await Required<global::HelpDesk.Domain.Models.Usuarios>(tech,ct)).Activo,"Tecnico inactivo.");
                var previous=await _unitOfWork.ListAsync(_unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().Query().Where(x=>x.IdTicket==t.IdTicket && x.FechaFin==null),ct);
                foreach(var a in previous)a.FechaFin=now;
                if(previous.Count>0)await _unitOfWork.Complete(ct); // liberar indice parcial antes del INSERT, dentro de la misma transaccion
                _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().Add(new global::HelpDesk.Domain.Models.AsignacionesTickets {IdAsignacionTicket=Guid.NewGuid(),IdTicket=t.IdTicket,IdEquipo=team,IdTecnico=tech,
                    IdAsignador=actor.Id,FechaAsignacion=now,Motivo=request.Datos.Motivo});
                t.IdEquipoAsignado=team;t.IdTecnicoAsignado=tech;t.Estado="asignado";
        History(t,"asignar",before,t.Estado,request.Datos.Motivo);
        _unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().Add(new global::HelpDesk.Domain.Models.Notificaciones {IdNotificacion=Guid.NewGuid(),IdUsuario=t.IdSolicitante,IdTicket=t.IdTicket,
            Titulo="Actualizacion de ticket",Mensaje=$"Ticket {t.Numero}: {t.Estado}",TipoEvento="ticket_actualizado",
            ClaveEvento=Guid.NewGuid().ToString("N"),FechaCreacion=now});
        await _unitOfWork.Complete(ct);
        return new(200,new TicketsResponseDto { IdTicket = t.IdTicket, Numero = t.Numero, IdSolicitante = t.IdSolicitante, IdCreador = t.IdCreador, IdAreaSolicitante = t.IdAreaSolicitante, IdServicio = t.IdServicio, IdEquipoAsignado = t.IdEquipoAsignado, IdTecnicoAsignado = t.IdTecnicoAsignado, IdPolitica = t.IdPolitica, Tipo = t.Tipo, Asunto = t.Asunto, Descripcion = t.Descripcion, Prioridad = t.Prioridad, Estado = t.Estado, MotivoEspera = t.MotivoEspera, Solucion = t.Solucion, MotivoCancelacion = t.MotivoCancelacion, MinutosPrimeraRespuesta = t.MinutosPrimeraRespuesta, MinutosResolucion = t.MinutosResolucion, FechaCreacion = t.FechaCreacion, FechaActualizacion = t.FechaActualizacion, FechaLimiteRespuesta = t.FechaLimiteRespuesta, FechaLimiteResolucion = t.FechaLimiteResolucion, FechaPrimeraRespuesta = t.FechaPrimeraRespuesta, FechaResolucion = t.FechaResolucion, FechaCierre = t.FechaCierre, FechaCancelacion = t.FechaCancelacion, Version = t.Version },"Accion completada.");

    }


    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
    private void History(global::HelpDesk.Domain.Models.Tickets ticket,string action,string? before,string? after,string reason) =>
        _unitOfWork.Repository<global::HelpDesk.Domain.Models.HistorialTickets>().Add(new global::HelpDesk.Domain.Models.HistorialTickets {IdHistorial=Guid.NewGuid(),IdTicket=ticket.IdTicket,IdUsuario=actor.Id,Accion=action,
            CampoModificado="estado",ValorAnterior=before,ValorNuevo=after,Motivo=reason,Fecha=DateTime.UtcNow});}
