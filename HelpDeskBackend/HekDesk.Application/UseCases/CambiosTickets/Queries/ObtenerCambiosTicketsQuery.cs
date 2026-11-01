using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CambiosTickets.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CambiosTickets;

public record ObtenerCambiosTicketsQuery(object[] Keys) : IRequest<ResponseDto<CambiosTicketsResponseDto>>;

internal sealed class ObtenerCambiosTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerCambiosTicketsQueryHandler> _logger) : IRequestHandler<ObtenerCambiosTicketsQuery, ResponseDto<CambiosTicketsResponseDto>>
{
    public async Task<ResponseDto<CambiosTicketsResponseDto>> Handle(ObtenerCambiosTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<Dictionary<string, object>>("CambiosTickets").FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new CambiosTicketsResponseDto() { IdCambio = (Guid)entity["IdCambio"], IdTicket = (Guid)entity["IdTicket"] });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerCambiosTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerCambiosTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
