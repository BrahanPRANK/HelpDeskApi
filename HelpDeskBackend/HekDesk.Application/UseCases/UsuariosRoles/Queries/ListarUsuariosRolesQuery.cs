using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public record ListarUsuariosRolesQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<UsuariosRolesResponseDto>>>;

internal sealed class ListarUsuariosRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarUsuariosRolesQueryHandler> _logger) : IRequestHandler<ListarUsuariosRolesQuery, ResponseDto<PaginaDto<UsuariosRolesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<UsuariosRolesResponseDto>>> Handle(ListarUsuariosRolesQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdUsuario).ThenBy(x => x.IdRol).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new UsuariosRolesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdRol = entity.IdRol,
                FechaAsignacion = entity.FechaAsignacion
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarUsuariosRolesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarUsuariosRolesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
