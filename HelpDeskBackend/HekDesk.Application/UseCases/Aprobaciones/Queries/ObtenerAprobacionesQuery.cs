using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Aprobaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones;

public record ObtenerAprobacionesQuery(object[] Keys) : IRequest<ResponseDto<AprobacionesResponseDto>>;

internal sealed class ObtenerAprobacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerAprobacionesQueryHandler> _logger) : IRequestHandler<ObtenerAprobacionesQuery, ResponseDto<AprobacionesResponseDto>>
{
    public async Task<ResponseDto<AprobacionesResponseDto>> Handle(ObtenerAprobacionesQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new AprobacionesResponseDto
            {
                IdAprobacion = entity.IdAprobacion,
                IdTicket = entity.IdTicket,
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdAprobador = entity.IdAprobador,
                Orden = entity.Orden,
                Motivo = entity.Motivo,
                Estado = entity.Estado,
                ComentarioDecision = entity.ComentarioDecision,
                FechaSolicitud = entity.FechaSolicitud,
                FechaDecision = entity.FechaDecision
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerAprobacionesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerAprobacionesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
