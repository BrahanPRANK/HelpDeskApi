using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.AsignacionesActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesActivos;

public class ListarAsignacionesActivosQuery : IRequest<ResponseDto<PaginaDto<AsignacionesActivosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
}

internal sealed class ListarAsignacionesActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAsignacionesActivosQuery, ResponseDto<PaginaDto<AsignacionesActivosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AsignacionesActivosResponseDto>>> Handle(ListarAsignacionesActivosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesActivos>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAsignacion).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new AsignacionesActivosResponseDto
            {
                IdAsignacion = entity.IdAsignacion,
                IdActivo = entity.IdActivo,
                IdUsuarioResponsable = entity.IdUsuarioResponsable,
                IdUsuarioAsignador = entity.IdUsuarioAsignador,
                FechaAsignacion = entity.FechaAsignacion,
                FechaDevolucion = entity.FechaDevolucion,
                Observaciones = entity.Observaciones
            })).ToListAsync();
            return new ResponseDto<PaginaDto<AsignacionesActivosResponseDto>>(200, new PaginaDto<AsignacionesActivosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<AsignacionesActivosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<AsignacionesActivosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
