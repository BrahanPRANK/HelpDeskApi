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

public class ListarPermisosQuery : IRequest<ResponseDto<PaginaDto<PermisosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
}

internal sealed class ListarPermisosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarPermisosQuery, ResponseDto<PaginaDto<PermisosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<PermisosResponseDto>>> Handle(ListarPermisosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Codigo.Contains(request.Buscar) || x.Modulo.Contains(request.Buscar) || x.Descripcion.Contains(request.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdPermiso).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new PermisosResponseDto
            {
                IdPermiso = entity.IdPermiso,
                Codigo = entity.Codigo,
                Modulo = entity.Modulo,
                Descripcion = entity.Descripcion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<PermisosResponseDto>>(200, new PaginaDto<PermisosResponseDto>(items, request.Pagina, request.Tamano, total));
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
