using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Cambios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios;

public record ActualizarCambiosCommand(object[] Keys, CambiosInputDto Datos) : IRequest<ResponseDto<CambiosResponseDto>>;

internal sealed class ActualizarCambiosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarCambiosCommandHandler> _logger) : IRequestHandler<ActualizarCambiosCommand, ResponseDto<CambiosResponseDto>>
{
    public async Task<ResponseDto<CambiosResponseDto>> Handle(ActualizarCambiosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Cambios>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(entity.Estado is "borrador" or "rechazado","Solo se edita un cambio en borrador o rechazado.",409);
            entity.IdResponsable = input.IdResponsable;
            entity.Titulo = input.Titulo;
            entity.Descripcion = input.Descripcion;
            entity.Riesgo = input.Riesgo;
            entity.PlanEjecucion = input.PlanEjecucion;
            entity.PlanReversion = input.PlanReversion;
            entity.RequiereAprobacion = input.RequiereAprobacion;
            entity.FechaProgramada = input.FechaProgramada;
            entity.Resultado = input.Resultado;

            await _unitOfWork.Complete(ct);
            return new(200,new CambiosResponseDto { IdCambio = entity.IdCambio, IdSolicitante = entity.IdSolicitante, IdResponsable = entity.IdResponsable, Titulo = entity.Titulo, Descripcion = entity.Descripcion, Riesgo = entity.Riesgo, PlanEjecucion = entity.PlanEjecucion, PlanReversion = entity.PlanReversion, RequiereAprobacion = entity.RequiereAprobacion, Estado = entity.Estado, FechaProgramada = entity.FechaProgramada, FechaFinalizacion = entity.FechaFinalizacion, Resultado = entity.Resultado, FechaCreacion = entity.FechaCreacion },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarCambiosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarCambiosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
