using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Cambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios;

public class CrearCambiosCommand : IRequest<ResponseDto<CambiosResponseDto>>
{
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

internal sealed class CrearCambiosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearCambiosCommandHandler> _logger) : IRequestHandler<CrearCambiosCommand, ResponseDto<CambiosResponseDto>>
{
    public async Task<ResponseDto<CambiosResponseDto>> Handle(CrearCambiosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Cambios()
            {
                IdCambio = Guid.NewGuid(),
                IdResponsable = request.IdResponsable,
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                Riesgo = request.Riesgo,
                PlanEjecucion = request.PlanEjecucion,
                PlanReversion = request.PlanReversion,
                RequiereAprobacion = request.RequiereAprobacion,
                FechaProgramada = request.FechaProgramada,
                Resultado = request.Resultado,
                FechaCreacion = DateTime.UtcNow,
                Estado = "borrador"
            };
            entity.IdSolicitante = actor.Id;
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Cambios>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new CambiosResponseDto
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
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearCambiosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearCambiosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
