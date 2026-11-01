using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Adjuntos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Adjuntos;

public record ActualizarAdjuntosCommand(object[] Keys, AdjuntosInputDto Datos) : IRequest<ResponseDto<AdjuntosResponseDto>>;

internal sealed class ActualizarAdjuntosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<ActualizarAdjuntosCommandHandler> _logger) : IRequestHandler<ActualizarAdjuntosCommand, ResponseDto<AdjuntosResponseDto>>
{
    public async Task<ResponseDto<AdjuntosResponseDto>> Handle(ActualizarAdjuntosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Adjuntos>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdTicket = input.IdTicket;
            entity.IdComentario = input.IdComentario;
            entity.NombreArchivo = input.NombreArchivo;
            entity.TipoArchivo = input.TipoArchivo;
            entity.TamanoBytes = input.TamanoBytes;
            entity.EsInterno = input.EsInterno;
            var attachedTicket=await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket,ct);access.EnsureRead(attachedTicket);
            if (entity.EsInterno) access.Permission("comentarios.internos");
            entity.IdUsuario=actor.Id;
            if (entity.IdComentario is Guid commentId)
            { var parent=await Required<global::HelpDesk.Domain.Models.Comentarios>(commentId,ct);Validation.Require(parent.IdTicket==entity.IdTicket,"El comentario pertenece entity otro ticket.");if(parent.EsInterno)entity.EsInterno=true; }

            await _unitOfWork.Complete(ct);
            return new(200,new AdjuntosResponseDto { IdAdjunto = entity.IdAdjunto, IdTicket = entity.IdTicket, IdComentario = entity.IdComentario, IdUsuario = entity.IdUsuario, NombreArchivo = entity.NombreArchivo, TipoArchivo = entity.TipoArchivo, TamanoBytes = entity.TamanoBytes, EsInterno = entity.EsInterno, Fecha = entity.Fecha },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarAdjuntosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarAdjuntosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
