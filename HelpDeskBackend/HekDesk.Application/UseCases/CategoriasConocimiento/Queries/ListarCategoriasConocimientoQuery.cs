using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CategoriasConocimiento;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public record ListarCategoriasConocimientoQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>>;

internal sealed class ListarCategoriasConocimientoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarCategoriasConocimientoQuery, ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>> Handle(ListarCategoriasConocimientoQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdCategoriaConocimiento).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new CategoriasConocimientoResponseDto
            {
                IdCategoriaConocimiento = entity.IdCategoriaConocimiento,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>(200, new PaginaDto<CategoriasConocimientoResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<CategoriasConocimientoResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
