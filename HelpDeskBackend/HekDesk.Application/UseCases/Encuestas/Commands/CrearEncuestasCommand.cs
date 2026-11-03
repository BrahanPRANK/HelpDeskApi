using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Encuestas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Encuestas;

public class CrearEncuestasCommand : IRequest<ResponseDto<EncuestasResponseDto>>
{
    public Guid IdTicket { get; set; }
    public short Calificacion { get; set; }
    public string? Comentario { get; set; }
}

internal sealed class CrearEncuestasCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor) : IRequestHandler<CrearEncuestasCommand, ResponseDto<EncuestasResponseDto>>
{
    public async Task<ResponseDto<EncuestasResponseDto>> Handle(CrearEncuestasCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            if (!access.Admin) access.Permission("encuestas.responder_propias");
            var entity = new global::HelpDesk.Domain.Models.Encuestas()
            {
                IdEncuesta = Guid.NewGuid(),
                IdTicket = request.IdTicket,
                Calificacion = request.Calificacion,
                Comentario = request.Comentario,
                FechaRespuesta = DateTime.UtcNow
            };
            var evaluated = await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket);
            Validation.Require(evaluated.IdSolicitante == actor.Id, "Solo el solicitante puede responder.", 403);
            Validation.Require(evaluated.Estado == "cerrado", "El ticket debe estar cerrado.", 409);
            entity.IdUsuario = actor.Id;

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Encuestas>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<EncuestasResponseDto>(201, new EncuestasResponseDto
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
            return new ResponseDto<EncuestasResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<EncuestasResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
