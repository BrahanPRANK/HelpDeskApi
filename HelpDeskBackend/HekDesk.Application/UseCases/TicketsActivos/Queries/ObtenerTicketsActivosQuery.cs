using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.TicketsActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsActivos;

public record ObtenerTicketsActivosQuery(object[] Keys) : IRequest<ResponseDto<TicketsActivosResponseDto>>;

internal sealed class ObtenerTicketsActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerTicketsActivosQueryHandler> _logger) : IRequestHandler<ObtenerTicketsActivosQuery, ResponseDto<TicketsActivosResponseDto>>
{
    public async Task<ResponseDto<TicketsActivosResponseDto>> Handle(ObtenerTicketsActivosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<Dictionary<string, object>>("TicketsActivos").GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new TicketsActivosResponseDto()
            {
                IdTicket = (Guid)entity["IdTicket"],
                IdActivo = (Guid)entity["IdActivo"]
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerTicketsActivosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerTicketsActivosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
