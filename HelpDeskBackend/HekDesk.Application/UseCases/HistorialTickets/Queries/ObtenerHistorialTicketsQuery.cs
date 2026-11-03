using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.HistorialTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.HistorialTickets;

public record ObtenerHistorialTicketsQuery(object[] Keys) : IRequest<ResponseDto<HistorialTicketsResponseDto>>;

internal sealed class ObtenerHistorialTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerHistorialTicketsQueryHandler> _logger) : IRequestHandler<ObtenerHistorialTicketsQuery, ResponseDto<HistorialTicketsResponseDto>>
{
    public async Task<ResponseDto<HistorialTicketsResponseDto>> Handle(ObtenerHistorialTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.HistorialTickets>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new HistorialTicketsResponseDto
            {
                IdHistorial = entity.IdHistorial,
                IdTicket = entity.IdTicket,
                IdUsuario = entity.IdUsuario,
                Accion = entity.Accion,
                CampoModificado = entity.CampoModificado,
                ValorAnterior = entity.ValorAnterior,
                ValorNuevo = entity.ValorNuevo,
                Motivo = entity.Motivo,
                Fecha = entity.Fecha
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerHistorialTicketsQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerHistorialTicketsQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
