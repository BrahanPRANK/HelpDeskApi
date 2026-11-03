using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesActivos.Commands;

public class DevolverActivoCommand : IRequest<ResponseDto<AccionResultadoDto>>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [StringLength(4000)]
    public string Motivo { get; set; } = string.Empty;
}

internal sealed class DevolverActivoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access)
    : IRequestHandler<DevolverActivoCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(DevolverActivoCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r);
            var result = "completado";
            access.Permission("activos.administrar");
            var assignment = await Required<global::HelpDesk.Domain.Models.AsignacionesActivos>(r.Id);
            Validation.Require(assignment.FechaDevolucion is null, "Ya fue devuelto.", 409);
            assignment.FechaDevolucion = DateTime.UtcNow;
            (await Required<global::HelpDesk.Domain.Models.Activos>(assignment.IdActivo)).Estado = "disponible";
            result = "devuelto";
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
