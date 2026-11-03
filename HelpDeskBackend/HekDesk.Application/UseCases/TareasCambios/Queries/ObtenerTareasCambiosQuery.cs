using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.TareasCambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TareasCambios;

public record ObtenerTareasCambiosQuery(object[] Keys) : IRequest<ResponseDto<TareasCambiosResponseDto>>;

internal sealed class ObtenerTareasCambiosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerTareasCambiosQueryHandler> _logger) : IRequestHandler<ObtenerTareasCambiosQuery, ResponseDto<TareasCambiosResponseDto>>
{
    public async Task<ResponseDto<TareasCambiosResponseDto>> Handle(ObtenerTareasCambiosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TareasCambios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new TareasCambiosResponseDto
            {
                IdTareaCambio = entity.IdTareaCambio,
                IdCambio = entity.IdCambio,
                IdResponsable = entity.IdResponsable,
                Orden = entity.Orden,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                Estado = entity.Estado,
                FechaCreacion = entity.FechaCreacion,
                FechaFinalizacion = entity.FechaFinalizacion
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerTareasCambiosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerTareasCambiosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
