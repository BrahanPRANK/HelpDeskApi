using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.RecuperacionesClave;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RecuperacionesClave;

public record ObtenerRecuperacionesClaveQuery(object[] Keys) : IRequest<ResponseDto<RecuperacionesClaveResponseDto>>;

internal sealed class ObtenerRecuperacionesClaveQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerRecuperacionesClaveQueryHandler> _logger) : IRequestHandler<ObtenerRecuperacionesClaveQuery, ResponseDto<RecuperacionesClaveResponseDto>>
{
    public async Task<ResponseDto<RecuperacionesClaveResponseDto>> Handle(ObtenerRecuperacionesClaveQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.RecuperacionesClave>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new RecuperacionesClaveResponseDto
            {
                IdRecuperacion = entity.IdRecuperacion,
                IdUsuario = entity.IdUsuario,
                FechaSolicitud = entity.FechaSolicitud,
                FechaVencimiento = entity.FechaVencimiento,
                FechaUso = entity.FechaUso
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerRecuperacionesClaveQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerRecuperacionesClaveQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
