using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ProblemasTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ProblemasTickets;

public record ObtenerProblemasTicketsQuery(object[] Keys) : IRequest<ResponseDto<ProblemasTicketsResponseDto>>;

internal sealed class ObtenerProblemasTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerProblemasTicketsQueryHandler> _logger) : IRequestHandler<ObtenerProblemasTicketsQuery, ResponseDto<ProblemasTicketsResponseDto>>
{
    public async Task<ResponseDto<ProblemasTicketsResponseDto>> Handle(ObtenerProblemasTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new ProblemasTicketsResponseDto()
            {
                IdProblema = (Guid)entity["IdProblema"],
                IdTicket = (Guid)entity["IdTicket"]
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerProblemasTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerProblemasTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
