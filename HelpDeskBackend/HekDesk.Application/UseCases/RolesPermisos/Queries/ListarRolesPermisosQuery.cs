using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.RolesPermisos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RolesPermisos;

public record ListarRolesPermisosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<RolesPermisosResponseDto>>>;

internal sealed class ListarRolesPermisosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarRolesPermisosQueryHandler> _logger) : IRequestHandler<ListarRolesPermisosQuery, ResponseDto<PaginaDto<RolesPermisosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<RolesPermisosResponseDto>>> Handle(ListarRolesPermisosQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<Dictionary<string, object>>("RolesPermisos").Query());

            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x["IdRol"]).ThenBy(x => x["IdPermiso"]).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new RolesPermisosResponseDto() { IdRol = (Guid)entity["IdRol"], IdPermiso = (Guid)entity["IdPermiso"] }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarRolesPermisosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarRolesPermisosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
