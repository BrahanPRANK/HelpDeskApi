using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Areas.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Areas;

public record ObtenerAreasQuery(object[] Keys) : IRequest<ResponseDto<AreasResponseDto>>;

internal sealed class ObtenerAreasQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerAreasQueryHandler> _logger) : IRequestHandler<ObtenerAreasQuery, ResponseDto<AreasResponseDto>>
{
    public async Task<ResponseDto<AreasResponseDto>> Handle(ObtenerAreasQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Areas>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new AreasResponseDto { IdArea = entity.IdArea, IdSede = entity.IdSede, Nombre = entity.Nombre, Activo = entity.Activo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerAreasQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerAreasQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
