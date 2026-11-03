using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TiposActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TiposActivos;

public record ListarTiposActivosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<TiposActivosResponseDto>>>;

internal sealed class ListarTiposActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarTiposActivosQuery, ResponseDto<PaginaDto<TiposActivosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<TiposActivosResponseDto>>> Handle(ListarTiposActivosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.TiposActivos>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdTipoActivo).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new TiposActivosResponseDto
            {
                IdTipoActivo = entity.IdTipoActivo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<TiposActivosResponseDto>>(200, new PaginaDto<TiposActivosResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<TiposActivosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<TiposActivosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
