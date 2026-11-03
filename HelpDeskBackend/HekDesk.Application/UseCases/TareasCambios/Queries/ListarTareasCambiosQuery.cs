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

public record ListarTareasCambiosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<TareasCambiosResponseDto>>>;

internal sealed class ListarTareasCambiosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarTareasCambiosQuery, ResponseDto<PaginaDto<TareasCambiosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<TareasCambiosResponseDto>>> Handle(ListarTareasCambiosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.TareasCambios>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Titulo.Contains(options.Buscar));
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdTareaCambio).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
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
            return new ResponseDto<PaginaDto<TareasCambiosResponseDto>>(200, new PaginaDto<TareasCambiosResponseDto>(items, options.Pagina, options.Tamano, total));
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
