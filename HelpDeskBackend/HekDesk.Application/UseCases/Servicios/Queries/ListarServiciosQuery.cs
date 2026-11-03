using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Servicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Servicios;

public record ListarServiciosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<ServiciosResponseDto>>>;

internal sealed class ListarServiciosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarServiciosQuery, ResponseDto<PaginaDto<ServiciosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ServiciosResponseDto>>> Handle(ListarServiciosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Servicios>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar) || x.Descripcion.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdServicio).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new ServiciosResponseDto
            {
                IdServicio = entity.IdServicio,
                IdEquipoResponsable = entity.IdEquipoResponsable,
                Nombre = entity.Nombre,
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Descripcion = entity.Descripcion,
                RequiereAprobacion = entity.RequiereAprobacion,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<ServiciosResponseDto>>(200, new PaginaDto<ServiciosResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<ServiciosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<ServiciosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
