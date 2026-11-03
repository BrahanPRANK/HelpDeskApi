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

public class ListarArticulosConocimientoQuery : IRequest<ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public string? Estado { get; set; }
}

internal sealed class ListarArticulosConocimientoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarArticulosConocimientoQuery, ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>> Handle(ListarArticulosConocimientoQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.ArticulosConocimiento>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Titulo.Contains(request.Buscar) || x.Contenido.Contains(request.Buscar));
            if (!string.IsNullOrWhiteSpace(request.Estado)) query = query.Where(x => x.Estado == request.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdArticulo).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
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
            return new ResponseDto<PaginaDto<ArticulosConocimientoResponseDto>>(200, new PaginaDto<ArticulosConocimientoResponseDto>(items, request.Pagina, request.Tamano, total));
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
