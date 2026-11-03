using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Adjuntos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Adjuntos;

public class ActualizarAdjuntosCommand : IRequest<ResponseDto<AdjuntosResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdTicket { get; set; }
    public Guid? IdComentario { get; set; }
    [Required]
    [StringLength(10000)]
    public string NombreArchivo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string TipoArchivo { get; set; } = "";
    public long TamanoBytes { get; set; }
    public bool EsInterno { get; set; }
}

internal sealed class ActualizarAdjuntosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor) : IRequestHandler<ActualizarAdjuntosCommand, ResponseDto<AdjuntosResponseDto>>
{
    public async Task<ResponseDto<AdjuntosResponseDto>> Handle(ActualizarAdjuntosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Adjuntos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdTicket = request.IdTicket;
            entity.IdComentario = request.IdComentario;
            entity.NombreArchivo = request.NombreArchivo;
            entity.TipoArchivo = request.TipoArchivo;
            entity.TamanoBytes = request.TamanoBytes;
            entity.EsInterno = request.EsInterno;
            var attachedTicket = await Required<global::HelpDesk.Domain.Models.Tickets>(entity.IdTicket);
            access.EnsureRead(attachedTicket);
            if (entity.EsInterno) access.Permission("comentarios.internos");
            entity.IdUsuario = actor.Id;
            if (entity.IdComentario is Guid commentId)
            {
                var parent = await Required<global::HelpDesk.Domain.Models.Comentarios>(commentId);
                Validation.Require(parent.IdTicket == entity.IdTicket, "El comentario pertenece entity otro ticket.");
                if (parent.EsInterno) entity.EsInterno = true;
            }

            await _unitOfWork.Complete();
            return new ResponseDto<AdjuntosResponseDto>(200, new AdjuntosResponseDto
            {
                IdAdjunto = entity.IdAdjunto,
                IdTicket = entity.IdTicket,
                IdComentario = entity.IdComentario,
                IdUsuario = entity.IdUsuario,
                NombreArchivo = entity.NombreArchivo,
                TipoArchivo = entity.TipoArchivo,
                TamanoBytes = entity.TamanoBytes,
                EsInterno = entity.EsInterno,
                Fecha = entity.Fecha
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AdjuntosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AdjuntosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
