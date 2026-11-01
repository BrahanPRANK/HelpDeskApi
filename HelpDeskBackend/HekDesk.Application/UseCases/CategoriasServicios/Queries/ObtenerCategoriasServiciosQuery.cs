using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CategoriasServicios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasServicios;

public record ObtenerCategoriasServiciosQuery(object[] Keys) : IRequest<ResponseDto<CategoriasServiciosResponseDto>>;

internal sealed class ObtenerCategoriasServiciosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerCategoriasServiciosQueryHandler> _logger) : IRequestHandler<ObtenerCategoriasServiciosQuery, ResponseDto<CategoriasServiciosResponseDto>>
{
    public async Task<ResponseDto<CategoriasServiciosResponseDto>> Handle(ObtenerCategoriasServiciosQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasServicios>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new CategoriasServiciosResponseDto { IdCategoriaServicio = entity.IdCategoriaServicio, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerCategoriasServiciosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerCategoriasServiciosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
