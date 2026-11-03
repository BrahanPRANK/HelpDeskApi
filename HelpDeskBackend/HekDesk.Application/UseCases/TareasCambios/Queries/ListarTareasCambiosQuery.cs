using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TareasCambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TareasCambios;

public class ListarTareasCambiosQuery : IRequest<ResponseDto<PaginaDto<TareasCambiosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public string? Estado { get; set; }
}

internal sealed class ListarTareasCambiosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarTareasCambiosQuery, ResponseDto<PaginaDto<TareasCambiosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<TareasCambiosResponseDto>>> Handle(ListarTareasCambiosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.TareasCambios>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Titulo.Contains(request.Buscar));
            if (!string.IsNullOrWhiteSpace(request.Estado)) query = query.Where(x => x.Estado == request.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdTareaCambio).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new TareasCambiosResponseDto
            {
                IdTareaCambio = entity.IdTareaCambio,
                IdCambio = entity.IdCambio,
                IdResponsable = entity.IdResponsable,
                Orden = entity.Orden,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                Estado = entity.Estado,
                FechaCreacion = entity.FechaCreacion,
                FechaFinalizacion = entity.FechaFinalizacion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<TareasCambiosResponseDto>>(200, new PaginaDto<TareasCambiosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<TareasCambiosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<TareasCambiosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
