using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.EquiposSoporte;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EquiposSoporte;

public class ListarEquiposSoporteQuery : IRequest<ResponseDto<PaginaDto<EquiposSoporteResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
}

internal sealed class ListarEquiposSoporteQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarEquiposSoporteQuery, ResponseDto<PaginaDto<EquiposSoporteResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<EquiposSoporteResponseDto>>> Handle(ListarEquiposSoporteQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Nombre.Contains(request.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEquipo).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new EquiposSoporteResponseDto
            {
                IdEquipo = entity.IdEquipo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<EquiposSoporteResponseDto>>(200, new PaginaDto<EquiposSoporteResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<EquiposSoporteResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<EquiposSoporteResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
