using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public class ListarUsuariosRolesQuery : IRequest<ResponseDto<PaginaDto<UsuariosRolesResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
}

internal sealed class ListarUsuariosRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarUsuariosRolesQuery, ResponseDto<PaginaDto<UsuariosRolesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<UsuariosRolesResponseDto>>> Handle(ListarUsuariosRolesQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdUsuario).ThenBy(x => x.IdRol).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new UsuariosRolesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdRol = entity.IdRol,
                FechaAsignacion = entity.FechaAsignacion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<UsuariosRolesResponseDto>>(200, new PaginaDto<UsuariosRolesResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<UsuariosRolesResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<UsuariosRolesResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
