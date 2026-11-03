using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Tickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Tickets;

public class ActualizarTicketsCommand : IRequest<ResponseDto<TicketsResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

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

internal sealed class ActualizarTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<ActualizarTicketsCommandHandler> _logger) : IRequestHandler<ActualizarTicketsCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(ActualizarTicketsCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            if (!access.Admin) access.Permission("tickets.crear");
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Tickets>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(request.Version == entity.Version, "La version del ticket cambio; vuelva a consultarlo.", 409);
            Validation.Require(entity.Estado == "nuevo" && (access.Admin || entity.IdSolicitante == actor.Id), "Solo se edita el texto de un ticket propio nuevo.", 403);
            Validation.Require(request.IdServicio == entity.IdServicio && request.Prioridad == entity.Prioridad && request.Tipo == entity.Tipo,
            "Este PUT solo cambia asunto y descripcion; servicio, prioridad y tipo se conservan.");
            entity.Asunto = request.Asunto;
            entity.Descripcion = request.Descripcion;

            entity.Version++;
            entity.FechaActualizacion = DateTime.UtcNow;
            await _unitOfWork.Complete();
            return new(200, new TicketsResponseDto
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
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarTicketsCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarTicketsCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
