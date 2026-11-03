using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.AsignacionesTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesTickets;

public record ObtenerAsignacionesTicketsQuery(object[] Keys) : IRequest<ResponseDto<AsignacionesTicketsResponseDto>>;

internal sealed class ObtenerAsignacionesTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerAsignacionesTicketsQueryHandler> _logger) : IRequestHandler<ObtenerAsignacionesTicketsQuery, ResponseDto<AsignacionesTicketsResponseDto>>
{
    public async Task<ResponseDto<AsignacionesTicketsResponseDto>> Handle(ObtenerAsignacionesTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new AsignacionesTicketsResponseDto
            {
                IdAsignacionTicket = entity.IdAsignacionTicket,
                IdTicket = entity.IdTicket,
                IdEquipo = entity.IdEquipo,
                IdTecnico = entity.IdTecnico,
                IdAsignador = entity.IdAsignador,
                FechaAsignacion = entity.FechaAsignacion,
                FechaFin = entity.FechaFin,
                Motivo = entity.Motivo
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerAsignacionesTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerAsignacionesTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
