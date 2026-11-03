using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CargaTecnicos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CargaTecnicos;

public class ListarCargaTecnicosQuery : IRequest<ResponseDto<PaginaDto<CargaTecnicosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
}

internal sealed class ListarCargaTecnicosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarCargaTecnicosQuery, ResponseDto<PaginaDto<CargaTecnicosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CargaTecnicosResponseDto>>> Handle(ListarCargaTecnicosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.CargaTecnicos>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEquipoAsignado).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new CargaTecnicosResponseDto
            {
                IdEquipoAsignado = entity.IdEquipoAsignado,
                IdTecnicoAsignado = entity.IdTecnicoAsignado,
                Prioridad = entity.Prioridad,
                TicketsPendientes = entity.TicketsPendientes
            })).ToListAsync();
            return new ResponseDto<PaginaDto<CargaTecnicosResponseDto>>(200, new PaginaDto<CargaTecnicosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<CargaTecnicosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<CargaTecnicosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
