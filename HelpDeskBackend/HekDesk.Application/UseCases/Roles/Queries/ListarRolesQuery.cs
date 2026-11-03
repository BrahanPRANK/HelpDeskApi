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

public record ListarRolesQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<RolesResponseDto>>>;

internal sealed class ListarRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarRolesQuery, ResponseDto<PaginaDto<RolesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<RolesResponseDto>>> Handle(ListarRolesQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdRol).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new RolesResponseDto
            {
                IdRol = entity.IdRol,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<RolesResponseDto>>(200, new PaginaDto<RolesResponseDto>(items, options.Pagina, options.Tamano, total));
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
