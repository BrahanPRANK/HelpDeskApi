using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ArticulosConocimiento;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ArticulosConocimiento;

public record ListarArticulosConocimientoQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>>;

internal sealed class ListarArticulosConocimientoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarArticulosConocimientoQuery, ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>> Handle(ListarArticulosConocimientoQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.ArticulosConocimiento>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Titulo.Contains(options.Buscar) || x.Contenido.Contains(options.Buscar));
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdArticulo).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new ArticulosConocimientoResponseDto
            {
                IdArticulo = entity.IdArticulo,
                IdAutor = entity.IdAutor,
                IdRevisor = entity.IdRevisor,
                IdCategoriaConocimiento = entity.IdCategoriaConocimiento,
                Titulo = entity.Titulo,
                Contenido = entity.Contenido,
                Estado = entity.Estado,
                EsInterno = entity.EsInterno,
                FechaCreacion = entity.FechaCreacion,
                FechaPublicacion = entity.FechaPublicacion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>(200, new PaginaDto<ArticulosConocimientoResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
