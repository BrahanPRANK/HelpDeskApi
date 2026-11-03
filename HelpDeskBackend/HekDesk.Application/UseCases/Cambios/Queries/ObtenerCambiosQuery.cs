using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Cambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios;

public record ObtenerCambiosQuery(object[] Keys) : IRequest<ResponseDto<CambiosResponseDto>>;

internal sealed class ObtenerCambiosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerCambiosQueryHandler> _logger) : IRequestHandler<ObtenerCambiosQuery, ResponseDto<CambiosResponseDto>>
{
    public async Task<ResponseDto<CambiosResponseDto>> Handle(ObtenerCambiosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Cambios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new CambiosResponseDto
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
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerCambiosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerCambiosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
