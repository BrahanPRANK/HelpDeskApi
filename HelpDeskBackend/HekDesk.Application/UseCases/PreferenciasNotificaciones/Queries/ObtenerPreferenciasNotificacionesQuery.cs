using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.PreferenciasNotificaciones.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones;

public record ObtenerPreferenciasNotificacionesQuery(object[] Keys) : IRequest<ResponseDto<PreferenciasNotificacionesResponseDto>>;

internal sealed class ObtenerPreferenciasNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerPreferenciasNotificacionesQueryHandler> _logger) : IRequestHandler<ObtenerPreferenciasNotificacionesQuery, ResponseDto<PreferenciasNotificacionesResponseDto>>
{
    public async Task<ResponseDto<PreferenciasNotificacionesResponseDto>> Handle(ObtenerPreferenciasNotificacionesQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new PreferenciasNotificacionesResponseDto { IdUsuario = entity.IdUsuario, TipoEvento = entity.TipoEvento, RecibirEnSistema = entity.RecibirEnSistema, RecibirPorCorreo = entity.RecibirPorCorreo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerPreferenciasNotificacionesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerPreferenciasNotificacionesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
