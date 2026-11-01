using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Comentarios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Comentarios;

public record ActualizarComentariosCommand(object[] Keys, ComentariosInputDto Datos) : IRequest<ResponseDto<ComentariosResponseDto>>;

internal sealed class ActualizarComentariosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<ActualizarComentariosCommandHandler> _logger) : IRequestHandler<ActualizarComentariosCommand, ResponseDto<ComentariosResponseDto>>
{
    public async Task<ResponseDto<ComentariosResponseDto>> Handle(ActualizarComentariosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Comentarios>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdTicket = input.IdTicket;
            entity.Mensaje = input.Mensaje;
            entity.EsInterno = input.EsInterno;
            var ticket=await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket,ct);access.EnsureRead(ticket);
            if (entity.EsInterno) access.Permission("comentarios.internos");
            Validation.Require(ticket.Estado is not ("cerrado" or "cancelado"),"No se agregan mensajes a un ticket finalizado.",409);
            Validation.Require(access.Admin || entity.IdAutor==actor.Id,"No puede editar mensajes ajenos.",403);


            await _unitOfWork.Complete(ct);
            return new(200,new ComentariosResponseDto { IdComentario = entity.IdComentario, IdTicket = entity.IdTicket, IdAutor = entity.IdAutor, Mensaje = entity.Mensaje, EsInterno = entity.EsInterno, Fecha = entity.Fecha },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarComentariosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarComentariosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
