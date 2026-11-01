using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Roles.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Roles;

public record ListarRolesQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<RolesResponseDto>>>;

internal sealed class ListarRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarRolesQueryHandler> _logger) : IRequestHandler<ListarRolesQuery, ResponseDto<PaginaDto<RolesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<RolesResponseDto>>> Handle(ListarRolesQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdRol).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new RolesResponseDto { IdRol = entity.IdRol, Nombre = entity.Nombre, Descripcion = entity.Descripcion }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarRolesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarRolesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
