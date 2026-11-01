using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.TicketsArticulos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsArticulos;

public record ObtenerTicketsArticulosQuery(object[] Keys) : IRequest<ResponseDto<TicketsArticulosResponseDto>>;

internal sealed class ObtenerTicketsArticulosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerTicketsArticulosQueryHandler> _logger) : IRequestHandler<ObtenerTicketsArticulosQuery, ResponseDto<TicketsArticulosResponseDto>>
{
    public async Task<ResponseDto<TicketsArticulosResponseDto>> Handle(ObtenerTicketsArticulosQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TicketsArticulos>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new TicketsArticulosResponseDto { IdTicket = entity.IdTicket, IdArticulo = entity.IdArticulo, IdUsuario = entity.IdUsuario, FechaVinculacion = entity.FechaVinculacion });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerTicketsArticulosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerTicketsArticulosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
