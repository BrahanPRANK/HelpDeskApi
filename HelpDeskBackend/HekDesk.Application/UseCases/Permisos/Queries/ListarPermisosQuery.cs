using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Permisos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Permisos;

public record ListarPermisosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<PermisosResponseDto>>>;

internal sealed class ListarPermisosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarPermisosQueryHandler> _logger) : IRequestHandler<ListarPermisosQuery, ResponseDto<PaginaDto<PermisosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<PermisosResponseDto>>> Handle(ListarPermisosQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Codigo.Contains(options.Buscar) || x.Modulo.Contains(options.Buscar) || x.Descripcion.Contains(options.Buscar));
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdPermiso).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new PermisosResponseDto { IdPermiso = entity.IdPermiso, Codigo = entity.Codigo, Modulo = entity.Modulo, Descripcion = entity.Descripcion }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarPermisosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarPermisosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
