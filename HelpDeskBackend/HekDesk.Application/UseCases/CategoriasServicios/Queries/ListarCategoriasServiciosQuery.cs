using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CategoriasServicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasServicios;

public class ListarCategoriasServiciosQuery : IRequest<ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
}

internal sealed class ListarCategoriasServiciosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarCategoriasServiciosQuery, ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>> Handle(ListarCategoriasServiciosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasServicios>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Nombre.Contains(request.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdCategoriaServicio).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new CategoriasServiciosResponseDto
            {
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>(200, new PaginaDto<CategoriasServiciosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<CategoriasServiciosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
