using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.CategoriasServicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasServicios;

public record ListarCategoriasServiciosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>>;

internal sealed class ListarCategoriasServiciosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarCategoriasServiciosQueryHandler> _logger) : IRequestHandler<ListarCategoriasServiciosQuery, ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>> Handle(ListarCategoriasServiciosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasServicios>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdCategoriaServicio).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new CategoriasServiciosResponseDto
            {
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarCategoriasServiciosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarCategoriasServiciosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
