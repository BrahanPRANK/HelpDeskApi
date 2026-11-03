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

public record ListarCargaTecnicosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<CargaTecnicosResponseDto>>>;

internal sealed class ListarCargaTecnicosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarCargaTecnicosQuery, ResponseDto<PaginaDto<CargaTecnicosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CargaTecnicosResponseDto>>> Handle(ListarCargaTecnicosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.CargaTecnicos>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEquipoAsignado).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new CargaTecnicosResponseDto
            {
                IdEquipoAsignado = entity.IdEquipoAsignado,
                IdTecnicoAsignado = entity.IdTecnicoAsignado,
                Prioridad = entity.Prioridad,
                TicketsPendientes = entity.TicketsPendientes
            })).ToListAsync();
            return new ResponseDto<PaginaDto<CargaTecnicosResponseDto>>(200, new PaginaDto<CargaTecnicosResponseDto>(items, options.Pagina, options.Tamano, total));
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
