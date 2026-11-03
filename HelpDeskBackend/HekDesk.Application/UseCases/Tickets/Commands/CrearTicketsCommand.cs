using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Tickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Tickets;

public class CrearTicketsCommand : IRequest<ResponseDto<TicketsResponseDto>>
{
    public Guid IdServicio { get; set; }
    [Required]
    [StringLength(10000)]
    public string Asunto { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Tipo { get; set; } = "incidente";
    [Required]
    [StringLength(10000)]
    public string Prioridad { get; set; } = "media";
    public int? Version { get; set; }
}

internal sealed class CrearTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor) : IRequestHandler<CrearTicketsCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(CrearTicketsCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            if (!access.Admin) access.Permission("tickets.crear");
            var entity = new global::HelpDesk.Domain.Models.Tickets()
            {
                IdTicket = Guid.NewGuid(),
                IdServicio = request.IdServicio,
                Asunto = request.Asunto,
                Descripcion = request.Descripcion,
                Tipo = request.Tipo,
                Prioridad = request.Prioridad,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow,
                FechaLimiteRespuesta = DateTime.UtcNow,
                FechaLimiteResolucion = DateTime.UtcNow
            };
            var user = await Required<global::HelpDesk.Domain.Models.Usuarios>(actor.Id);
            Validation.Require(user.IdArea.HasValue, "Asigne un area al usuario antes de crear tickets.");
            var service = await Required<global::HelpDesk.Domain.Models.Servicios>(entity.IdServicio);
            Validation.Require(service.Activo, "El servicio esta inactivo.");
            var policy = (await (_unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>().Query().Where(x => x.Prioridad == entity.Prioridad && x.Activo)).ToListAsync()).SingleOrDefault()
            ?? throw new BusinessException(400, "Configure una politica activa para esta prioridad.");
            entity.IdSolicitante = actor.Id;
            entity.IdCreador = actor.Id;
            entity.IdAreaSolicitante = user.IdArea!.Value;
            entity.IdEquipoAsignado = service.IdEquipoResponsable;
            entity.IdTecnicoAsignado = null;
            entity.Estado = "nuevo";
            entity.IdPolitica = policy.IdPolitica;
            entity.MinutosPrimeraRespuesta = policy.MinutosPrimeraRespuesta;
            entity.MinutosResolucion = policy.MinutosResolucion;
            entity.FechaLimiteRespuesta = entity.FechaCreacion.AddMinutes(policy.MinutosPrimeraRespuesta);
            entity.FechaLimiteResolucion = entity.FechaCreacion.AddMinutes(policy.MinutosResolucion);
            entity.Version = 1;

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Tickets>().AddAsync(entity);
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().AddAsync(new global::HelpDesk.Domain.Models.AsignacionesTickets
            {
                IdAsignacionTicket = Guid.NewGuid(),

                IdTicket = entity.IdTicket,

                IdEquipo = entity.IdEquipoAsignado,

                IdAsignador = actor.Id,

                FechaAsignacion = DateTime.UtcNow,

                Motivo = "Registro inicial"
            });
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.HistorialTickets>().AddAsync(new global::HelpDesk.Domain.Models.HistorialTickets
            {
                IdHistorial = Guid.NewGuid(),

                IdTicket = entity.IdTicket,

                IdUsuario = actor.Id,

                Accion = "creado",

                CampoModificado = "estado",

                ValorNuevo = entity.Estado,

                Motivo = "Registro del ticket",

                Fecha = DateTime.UtcNow
            });
            await _unitOfWork.Complete();
            return new ResponseDto<TicketsResponseDto>(201, new TicketsResponseDto
            {
                IdTicket = entity.IdTicket,
                Numero = entity.Numero,
                IdSolicitante = entity.IdSolicitante,
                IdCreador = entity.IdCreador,
                IdAreaSolicitante = entity.IdAreaSolicitante,
                IdServicio = entity.IdServicio,
                IdEquipoAsignado = entity.IdEquipoAsignado,
                IdTecnicoAsignado = entity.IdTecnicoAsignado,
                IdPolitica = entity.IdPolitica,
                Tipo = entity.Tipo,
                Asunto = entity.Asunto,
                Descripcion = entity.Descripcion,
                Prioridad = entity.Prioridad,
                Estado = entity.Estado,
                MotivoEspera = entity.MotivoEspera,
                Solucion = entity.Solucion,
                MotivoCancelacion = entity.MotivoCancelacion,
                MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta,
                MinutosResolucion = entity.MinutosResolucion,
                FechaCreacion = entity.FechaCreacion,
                FechaActualizacion = entity.FechaActualizacion,
                FechaLimiteRespuesta = entity.FechaLimiteRespuesta,
                FechaLimiteResolucion = entity.FechaLimiteResolucion,
                FechaPrimeraRespuesta = entity.FechaPrimeraRespuesta,
                FechaResolucion = entity.FechaResolucion,
                FechaCierre = entity.FechaCierre,
                FechaCancelacion = entity.FechaCancelacion,
                Version = entity.Version
            }, "Registro creado.");
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

}
