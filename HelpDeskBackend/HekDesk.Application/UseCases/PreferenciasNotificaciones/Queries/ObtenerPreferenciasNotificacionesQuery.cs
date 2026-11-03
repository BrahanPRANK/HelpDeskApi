using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PreferenciasNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones;

public record ObtenerPreferenciasNotificacionesQuery(object[] Keys) : IRequest<ResponseDto<PreferenciasNotificacionesResponseDto>>;

internal sealed class ObtenerPreferenciasNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerPreferenciasNotificacionesQuery, ResponseDto<PreferenciasNotificacionesResponseDto>>
{
    public async Task<ResponseDto<PreferenciasNotificacionesResponseDto>> Handle(ObtenerPreferenciasNotificacionesQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<PreferenciasNotificacionesResponseDto>(200, new PreferenciasNotificacionesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                TipoEvento = entity.TipoEvento,
                RecibirEnSistema = entity.RecibirEnSistema,
                RecibirPorCorreo = entity.RecibirPorCorreo
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PreferenciasNotificacionesResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PreferenciasNotificacionesResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
