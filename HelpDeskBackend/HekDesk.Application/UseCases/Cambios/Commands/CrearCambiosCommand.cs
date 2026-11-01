using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Cambios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios;

public record CrearCambiosCommand(CambiosInputDto Datos) : IRequest<ResponseDto<CambiosResponseDto>>;

internal sealed class CrearCambiosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearCambiosCommandHandler> _logger) : IRequestHandler<CrearCambiosCommand, ResponseDto<CambiosResponseDto>>
{
    public async Task<ResponseDto<CambiosResponseDto>> Handle(CrearCambiosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.Cambios() { IdCambio = Guid.NewGuid(), IdResponsable = input.IdResponsable, Titulo = input.Titulo, Descripcion = input.Descripcion, Riesgo = input.Riesgo, PlanEjecucion = input.PlanEjecucion, PlanReversion = input.PlanReversion, RequiereAprobacion = input.RequiereAprobacion, FechaProgramada = input.FechaProgramada, Resultado = input.Resultado, FechaCreacion = DateTime.UtcNow, Estado = "borrador" };
            entity.IdSolicitante=actor.Id;
            _unitOfWork.Repository<global::HelpDesk.Domain.Models.Cambios>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new CambiosResponseDto { IdCambio = entity.IdCambio, IdSolicitante = entity.IdSolicitante, IdResponsable = entity.IdResponsable, Titulo = entity.Titulo, Descripcion = entity.Descripcion, Riesgo = entity.Riesgo, PlanEjecucion = entity.PlanEjecucion, PlanReversion = entity.PlanReversion, RequiereAprobacion = entity.RequiereAprobacion, Estado = entity.Estado, FechaProgramada = entity.FechaProgramada, FechaFinalizacion = entity.FechaFinalizacion, Resultado = entity.Resultado, FechaCreacion = entity.FechaCreacion },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearCambiosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearCambiosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
