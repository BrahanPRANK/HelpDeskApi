using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Adjuntos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Adjuntos;

public record CrearAdjuntosCommand(AdjuntosInputDto Datos) : IRequest<ResponseDto<AdjuntosResponseDto>>;

internal sealed class CrearAdjuntosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearAdjuntosCommandHandler> _logger) : IRequestHandler<CrearAdjuntosCommand, ResponseDto<AdjuntosResponseDto>>
{
    public async Task<ResponseDto<AdjuntosResponseDto>> Handle(CrearAdjuntosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.Adjuntos() { IdAdjunto = Guid.NewGuid(), IdTicket = input.IdTicket, IdComentario = input.IdComentario, NombreArchivo = input.NombreArchivo, TipoArchivo = input.TipoArchivo, TamanoBytes = input.TamanoBytes, EsInterno = input.EsInterno, Fecha = DateTime.UtcNow };
            var attachedTicket=await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket,ct);access.EnsureRead(attachedTicket);
            if (entity.EsInterno) access.Permission("comentarios.internos");
            entity.IdUsuario=actor.Id;
            if (entity.IdComentario is Guid commentId)
            { var parent=await Required<global::HelpDesk.Domain.Models.Comentarios>(commentId,ct);Validation.Require(parent.IdTicket==entity.IdTicket,"El comentario pertenece entity otro ticket.");if(parent.EsInterno)entity.EsInterno=true; }

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.Adjuntos>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new AdjuntosResponseDto { IdAdjunto = entity.IdAdjunto, IdTicket = entity.IdTicket, IdComentario = entity.IdComentario, IdUsuario = entity.IdUsuario, NombreArchivo = entity.NombreArchivo, TipoArchivo = entity.TipoArchivo, TamanoBytes = entity.TamanoBytes, EsInterno = entity.EsInterno, Fecha = entity.Fecha },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearAdjuntosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearAdjuntosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
