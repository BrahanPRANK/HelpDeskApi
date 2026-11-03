using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Cambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios;

public class ActualizarCambiosCommand : IRequest<ResponseDto<CambiosResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdResponsable { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Riesgo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string PlanEjecucion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string PlanReversion { get; set; } = "";
    public bool RequiereAprobacion { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public string? Resultado { get; set; }
}

internal sealed class ActualizarCambiosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ActualizarCambiosCommand, ResponseDto<CambiosResponseDto>>
{
    public async Task<ResponseDto<CambiosResponseDto>> Handle(ActualizarCambiosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Cambios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(entity.Estado is "borrador" or "rechazado", "Solo se edita un cambio en borrador o rechazado.", 409);
            entity.IdResponsable = request.IdResponsable;
            entity.Titulo = request.Titulo;
            entity.Descripcion = request.Descripcion;
            entity.Riesgo = request.Riesgo;
            entity.PlanEjecucion = request.PlanEjecucion;
            entity.PlanReversion = request.PlanReversion;
            entity.RequiereAprobacion = request.RequiereAprobacion;
            entity.FechaProgramada = request.FechaProgramada;
            entity.Resultado = request.Resultado;

            await _unitOfWork.Complete();
            return new ResponseDto<CambiosResponseDto>(200, new CambiosResponseDto
            {
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdResponsable = entity.IdResponsable,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                Riesgo = entity.Riesgo,
                PlanEjecucion = entity.PlanEjecucion,
                PlanReversion = entity.PlanReversion,
                RequiereAprobacion = entity.RequiereAprobacion,
                Estado = entity.Estado,
                FechaProgramada = entity.FechaProgramada,
                FechaFinalizacion = entity.FechaFinalizacion,
                Resultado = entity.Resultado,
                FechaCreacion = entity.FechaCreacion
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<CambiosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<CambiosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
