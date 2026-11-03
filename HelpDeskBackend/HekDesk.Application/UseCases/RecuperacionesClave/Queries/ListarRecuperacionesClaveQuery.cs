using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.RecuperacionesClave;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RecuperacionesClave;

public class ListarRecuperacionesClaveQuery : IRequest<ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
}

internal sealed class ListarRecuperacionesClaveQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarRecuperacionesClaveQuery, ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>> Handle(ListarRecuperacionesClaveQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.RecuperacionesClave>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdRecuperacion).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new RecuperacionesClaveResponseDto
            {
                IdRecuperacion = entity.IdRecuperacion,
                IdUsuario = entity.IdUsuario,
                FechaSolicitud = entity.FechaSolicitud,
                FechaVencimiento = entity.FechaVencimiento,
                FechaUso = entity.FechaUso
            })).ToListAsync();
            return new ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>(200, new PaginaDto<RecuperacionesClaveResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
