using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Comentarios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Comentarios;

public class CrearComentariosCommand : IRequest<ResponseDto<ComentariosResponseDto>>
{
    public Guid IdTicket { get; set; }
    [Required]
    [StringLength(10000)]
    public string Mensaje { get; set; } = "";
    public bool EsInterno { get; set; }
}

internal sealed class CrearComentariosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor) : IRequestHandler<CrearComentariosCommand, ResponseDto<ComentariosResponseDto>>
{
    public async Task<ResponseDto<ComentariosResponseDto>> Handle(CrearComentariosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            if (!access.Admin) access.Permission("comentarios.responder");
            var entity = new global::HelpDesk.Domain.Models.Comentarios()
            {
                IdComentario = Guid.NewGuid(),
                IdTicket = request.IdTicket,
                Mensaje = request.Mensaje,
                EsInterno = request.EsInterno,
                Fecha = DateTime.UtcNow
            };
            var ticket = await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket);
            access.EnsureRead(ticket);
            if (entity.EsInterno) access.Permission("comentarios.internos");
            Validation.Require(ticket.Estado is not ("cerrado" or "cancelado"), "No se agregan mensajes a un ticket finalizado.", 409);

            entity.IdAutor = actor.Id;
            if (!entity.EsInterno && actor.HasPermission("tickets.atender") && ticket.IdSolicitante != actor.Id)
            {
                access.EnsureWork(ticket);
                ticket.FechaPrimeraRespuesta ??= DateTime.UtcNow;
            }

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Comentarios>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<ComentariosResponseDto>(201, new ComentariosResponseDto
            {
                IdComentario = entity.IdComentario,
                IdTicket = entity.IdTicket,
                IdAutor = entity.IdAutor,
                Mensaje = entity.Mensaje,
                EsInterno = entity.EsInterno,
                Fecha = entity.Fecha
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ComentariosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ComentariosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
