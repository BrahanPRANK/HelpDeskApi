using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CambiosTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CambiosTickets;

public record ObtenerCambiosTicketsQuery(object[] Keys) : IRequest<ResponseDto<CambiosTicketsResponseDto>>;

internal sealed class ObtenerCambiosTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerCambiosTicketsQuery, ResponseDto<CambiosTicketsResponseDto>>
{
    public async Task<ResponseDto<CambiosTicketsResponseDto>> Handle(ObtenerCambiosTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Dictionary<string, object>>("CambiosTickets").GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<CambiosTicketsResponseDto>(200, new CambiosTicketsResponseDto()
            {
                IdCambio = (Guid)entity["IdCambio"],
                IdTicket = (Guid)entity["IdTicket"]
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<CambiosTicketsResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<CambiosTicketsResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
