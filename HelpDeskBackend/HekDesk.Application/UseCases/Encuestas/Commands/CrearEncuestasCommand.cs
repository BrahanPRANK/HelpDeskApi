using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Encuestas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Encuestas;

public record CrearEncuestasCommand(EncuestasInputDto Datos) : IRequest<ResponseDto<EncuestasResponseDto>>;

internal sealed class CrearEncuestasCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearEncuestasCommandHandler> _logger) : IRequestHandler<CrearEncuestasCommand, ResponseDto<EncuestasResponseDto>>
{
    public async Task<ResponseDto<EncuestasResponseDto>> Handle(CrearEncuestasCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            if (!access.Admin) access.Permission("encuestas.responder_propias");
            var entity = new global::HelpDesk.Domain.Models.Encuestas()
            {
                IdEncuesta = Guid.NewGuid(),
                IdTicket = input.IdTicket,
                Calificacion = input.Calificacion,
                Comentario = input.Comentario,
                FechaRespuesta = DateTime.UtcNow
            };
            var evaluated = await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket);
            Validation.Require(evaluated.IdSolicitante == actor.Id, "Solo el solicitante puede responder.", 403);
            Validation.Require(evaluated.Estado == "cerrado", "El ticket debe estar cerrado.", 409);
            entity.IdUsuario = actor.Id;

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Encuestas>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new EncuestasResponseDto
            {
                IdEncuesta = entity.IdEncuesta,
                IdTicket = entity.IdTicket,
                IdUsuario = entity.IdUsuario,
                Calificacion = entity.Calificacion,
                Comentario = entity.Comentario,
                FechaRespuesta = entity.FechaRespuesta
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearEncuestasCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearEncuestasCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
