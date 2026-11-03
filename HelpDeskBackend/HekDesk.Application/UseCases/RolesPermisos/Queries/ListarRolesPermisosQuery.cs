using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.RolesPermisos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RolesPermisos;

public class ListarRolesPermisosQuery : IRequest<ResponseDto<PaginaDto<RolesPermisosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
}

internal sealed class ListarRolesPermisosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarRolesPermisosQuery, ResponseDto<PaginaDto<RolesPermisosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<RolesPermisosResponseDto>>> Handle(ListarRolesPermisosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<Dictionary<string, object>>("RolesPermisos").Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x["IdRol"]).ThenBy(x => x["IdPermiso"]).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new RolesPermisosResponseDto()
            {
                IdRol = (Guid)entity["IdRol"],
                IdPermiso = (Guid)entity["IdPermiso"]
            })).ToListAsync();
            return new ResponseDto<PaginaDto<RolesPermisosResponseDto>>(200, new PaginaDto<RolesPermisosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<RolesPermisosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<RolesPermisosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
