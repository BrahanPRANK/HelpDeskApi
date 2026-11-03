using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Permisos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Permisos;

public record ListarPermisosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<PermisosResponseDto>>>;

internal sealed class ListarPermisosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarPermisosQuery, ResponseDto<PaginaDto<PermisosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<PermisosResponseDto>>> Handle(ListarPermisosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Codigo.Contains(options.Buscar) || x.Modulo.Contains(options.Buscar) || x.Descripcion.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdPermiso).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new PermisosResponseDto
            {
                IdPermiso = entity.IdPermiso,
                Codigo = entity.Codigo,
                Modulo = entity.Modulo,
                Descripcion = entity.Descripcion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<PermisosResponseDto>>(200, new PaginaDto<PermisosResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<PermisosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<PermisosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
