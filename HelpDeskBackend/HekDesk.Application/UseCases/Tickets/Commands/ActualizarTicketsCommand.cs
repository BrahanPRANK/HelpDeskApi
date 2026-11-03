using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Tickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Tickets;

public record ActualizarTicketsCommand(object[] Keys, TicketsInputDto Datos) : IRequest<ResponseDto<TicketsResponseDto>>;

internal sealed class ActualizarTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<ActualizarTicketsCommandHandler> _logger) : IRequestHandler<ActualizarTicketsCommand, ResponseDto<TicketsResponseDto>>
{
    public async Task<ResponseDto<TicketsResponseDto>> Handle(ActualizarTicketsCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            if (!access.Admin) access.Permission("tickets.crear");
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Tickets>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(input.Version == entity.Version, "La version del ticket cambio; vuelva a consultarlo.", 409);
            Validation.Require(entity.Estado == "nuevo" && (access.Admin || entity.IdSolicitante == actor.Id), "Solo se edita el texto de un ticket propio nuevo.", 403);
            Validation.Require(input.IdServicio == entity.IdServicio && input.Prioridad == entity.Prioridad && input.Tipo == entity.Tipo,
            "Este PUT solo cambia asunto y descripcion; servicio, prioridad y tipo se conservan.");
            entity.Asunto = input.Asunto;
            entity.Descripcion = input.Descripcion;

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
