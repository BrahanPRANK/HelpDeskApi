using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TicketsActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsActivos;

public class ObtenerTicketsActivosQuery : IRequest<ResponseDto<TicketsActivosResponseDto>>
{
    public Guid IdTicket { get; set; }
    public Guid IdActivo { get; set; }
}

internal sealed class ObtenerTicketsActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerTicketsActivosQuery, ResponseDto<TicketsActivosResponseDto>>
{
    public async Task<ResponseDto<TicketsActivosResponseDto>> Handle(ObtenerTicketsActivosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Dictionary<string, object>>("TicketsActivos").GetByIdAsync(request.IdTicket, request.IdActivo)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<TicketsActivosResponseDto>(200, new TicketsActivosResponseDto()
            {
                IdTicket = (Guid)entity["IdTicket"],
                IdActivo = (Guid)entity["IdActivo"]
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<TicketsActivosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<TicketsActivosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
