using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TicketsArticulos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsArticulos;

public record ObtenerTicketsArticulosQuery(object[] Keys) : IRequest<ResponseDto<TicketsArticulosResponseDto>>;

internal sealed class ObtenerTicketsArticulosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerTicketsArticulosQuery, ResponseDto<TicketsArticulosResponseDto>>
{
    public async Task<ResponseDto<TicketsArticulosResponseDto>> Handle(ObtenerTicketsArticulosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TicketsArticulos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<TicketsArticulosResponseDto>(200, new TicketsArticulosResponseDto
            {
                IdTicket = entity.IdTicket,
                IdArticulo = entity.IdArticulo,
                IdUsuario = entity.IdUsuario,
                FechaVinculacion = entity.FechaVinculacion
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<TicketsArticulosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<TicketsArticulosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
