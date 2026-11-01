using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CategoriasConocimiento.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public record ListarCategoriasConocimientoQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>>;

internal sealed class ListarCategoriasConocimientoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarCategoriasConocimientoQueryHandler> _logger) : IRequestHandler<ListarCategoriasConocimientoQuery, ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>> Handle(ListarCategoriasConocimientoQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdCategoriaConocimiento).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new CategoriasConocimientoResponseDto { IdCategoriaConocimiento = entity.IdCategoriaConocimiento, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarCategoriasConocimientoQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarCategoriasConocimientoQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
