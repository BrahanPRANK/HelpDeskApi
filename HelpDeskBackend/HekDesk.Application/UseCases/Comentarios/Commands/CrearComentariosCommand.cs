using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Comentarios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Comentarios;

public record CrearComentariosCommand(ComentariosInputDto Datos) : IRequest<ResponseDto<ComentariosResponseDto>>;

internal sealed class CrearComentariosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearComentariosCommandHandler> _logger) : IRequestHandler<CrearComentariosCommand, ResponseDto<ComentariosResponseDto>>
{
    public async Task<ResponseDto<ComentariosResponseDto>> Handle(CrearComentariosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            if (!access.Admin) access.Permission("comentarios.responder");
            var entity=new global::HelpDesk.Domain.Models.Comentarios() { IdComentario = Guid.NewGuid(), IdTicket = input.IdTicket, Mensaje = input.Mensaje, EsInterno = input.EsInterno, Fecha = DateTime.UtcNow };
            var ticket=await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket,ct);access.EnsureRead(ticket);
            if (entity.EsInterno) access.Permission("comentarios.internos");
            Validation.Require(ticket.Estado is not ("cerrado" or "cancelado"),"No se agregan mensajes a un ticket finalizado.",409);

            entity.IdAutor=actor.Id;
            if ( !entity.EsInterno && actor.HasPermission("tickets.atender") && ticket.IdSolicitante!=actor.Id)
            { access.EnsureWork(ticket);ticket.FechaPrimeraRespuesta ??= DateTime.UtcNow; }

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.Comentarios>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new ComentariosResponseDto { IdComentario = entity.IdComentario, IdTicket = entity.IdTicket, IdAutor = entity.IdAutor, Mensaje = entity.Mensaje, EsInterno = entity.EsInterno, Fecha = entity.Fecha },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearComentariosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearComentariosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
