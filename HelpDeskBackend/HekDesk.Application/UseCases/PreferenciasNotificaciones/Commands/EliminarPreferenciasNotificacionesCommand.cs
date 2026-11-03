using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PreferenciasNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones;

public record EliminarPreferenciasNotificacionesCommand(object[] Keys) : IRequest<ResponseDto<OperacionDto>>;

internal sealed class EliminarPreferenciasNotificacionesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarPreferenciasNotificacionesCommandHandler> _logger) : IRequestHandler<EliminarPreferenciasNotificacionesCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarPreferenciasNotificacionesCommand request, CancellationToken ct)
    {
        try
        {
            if (!access.Admin) access.Permission("notificaciones.propias");
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>(null).Delete(entity);
            await _unitOfWork.Complete();
            return new(200, new(true), "Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarPreferenciasNotificacionesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarPreferenciasNotificacionesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
