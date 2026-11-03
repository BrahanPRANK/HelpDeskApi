using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ProblemasTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ProblemasTickets;

public record ObtenerProblemasTicketsQuery(object[] Keys) : IRequest<ResponseDto<ProblemasTicketsResponseDto>>;

internal sealed class ObtenerProblemasTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerProblemasTicketsQuery, ResponseDto<ProblemasTicketsResponseDto>>
{
    public async Task<ResponseDto<ProblemasTicketsResponseDto>> Handle(ObtenerProblemasTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<ProblemasTicketsResponseDto>(200, new ProblemasTicketsResponseDto()
            {
                IdProblema = (Guid)entity["IdProblema"],
                IdTicket = (Guid)entity["IdTicket"]
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ProblemasTicketsResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ProblemasTicketsResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
