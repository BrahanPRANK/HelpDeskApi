using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones.Commands;

public class AprobarSolicitudCommand : IRequest<ResponseDto<AccionResultadoDto>>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [StringLength(4000)]
    public string Motivo { get; set; } = string.Empty;
}

internal sealed class AprobarSolicitudCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access)
    : IRequestHandler<AprobarSolicitudCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(AprobarSolicitudCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r);
            var result = "completado";
            access.Permission("aprobaciones.decidir");
            var a = await Required<global::HelpDesk.Domain.Models.Aprobaciones>(r.Id);
            Validation.Require(a.IdAprobador == actor.Id, "Solo el aprobador asignado puede decidir.", 403);
            Validation.Require(a.Estado == "pendiente", "Ya fue decidida.", 409);
            var previous = await (_unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().Query().Where(x => x.IdTicket == a.IdTicket && x.IdCambio == a.IdCambio && x.Orden < a.Orden)).ToListAsync();
            Validation.Require(previous.All(x => x.Estado == "aprobado"), "Complete los pasos anteriores en orden.", 409);
            Validation.Require(!string.IsNullOrWhiteSpace(r.Motivo), "Indique el motivo de la decision.");
            a.Estado = "aprobado";
            a.FechaDecision = DateTime.UtcNow;
            a.ComentarioDecision = r.Motivo;
            result = a.Estado;
            await _unitOfWork.Complete();
            return new ResponseDto<AccionResultadoDto>(200, new AccionResultadoDto(r.Id, result));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AccionResultadoDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AccionResultadoDto>(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
