using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CategoriasConocimiento.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public record ObtenerCategoriasConocimientoQuery(object[] Keys) : IRequest<ResponseDto<CategoriasConocimientoResponseDto>>;

internal sealed class ObtenerCategoriasConocimientoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerCategoriasConocimientoQueryHandler> _logger) : IRequestHandler<ObtenerCategoriasConocimientoQuery, ResponseDto<CategoriasConocimientoResponseDto>>
{
    public async Task<ResponseDto<CategoriasConocimientoResponseDto>> Handle(ObtenerCategoriasConocimientoQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new CategoriasConocimientoResponseDto { IdCategoriaConocimiento = entity.IdCategoriaConocimiento, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerCategoriasConocimientoQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerCategoriasConocimientoQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
