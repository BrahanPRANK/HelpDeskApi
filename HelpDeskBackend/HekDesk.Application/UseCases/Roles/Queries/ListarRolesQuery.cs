using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Roles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Roles;

public class ListarRolesQuery : IRequest<ResponseDto<PaginaDto<RolesResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
}

internal sealed class ListarRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarRolesQuery, ResponseDto<PaginaDto<RolesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<RolesResponseDto>>> Handle(ListarRolesQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Nombre.Contains(request.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdRol).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new RolesResponseDto
            {
                IdRol = entity.IdRol,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<RolesResponseDto>>(200, new PaginaDto<RolesResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<RolesResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<RolesResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
