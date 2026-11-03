using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.TiposActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TiposActivos;

public record ObtenerTiposActivosQuery(object[] Keys) : IRequest<ResponseDto<TiposActivosResponseDto>>;

internal sealed class ObtenerTiposActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerTiposActivosQueryHandler> _logger) : IRequestHandler<ObtenerTiposActivosQuery, ResponseDto<TiposActivosResponseDto>>
{
    public async Task<ResponseDto<TiposActivosResponseDto>> Handle(ObtenerTiposActivosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TiposActivos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new TiposActivosResponseDto
            {
                IdTipoActivo = entity.IdTipoActivo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerTiposActivosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerTiposActivosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
