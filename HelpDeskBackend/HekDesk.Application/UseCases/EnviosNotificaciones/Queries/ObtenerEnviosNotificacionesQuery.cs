using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.EnviosNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EnviosNotificaciones;

public record ObtenerEnviosNotificacionesQuery(object[] Keys) : IRequest<ResponseDto<EnviosNotificacionesResponseDto>>;

internal sealed class ObtenerEnviosNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerEnviosNotificacionesQuery, ResponseDto<EnviosNotificacionesResponseDto>>
{
    public async Task<ResponseDto<EnviosNotificacionesResponseDto>> Handle(ObtenerEnviosNotificacionesQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.EnviosNotificaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<EnviosNotificacionesResponseDto>(200, new EnviosNotificacionesResponseDto
            {
                IdEnvio = entity.IdEnvio,
                IdNotificacion = entity.IdNotificacion,
                CorreoDestino = entity.CorreoDestino,
                Estado = entity.Estado,
                Intentos = entity.Intentos,
                MaximoIntentos = entity.MaximoIntentos,
                FechaCreacion = entity.FechaCreacion,
                FechaProximoIntento = entity.FechaProximoIntento,
                BloqueoHasta = entity.BloqueoHasta,
                FechaEnvio = entity.FechaEnvio,
                IdentificadorProveedor = entity.IdentificadorProveedor
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<EnviosNotificacionesResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<EnviosNotificacionesResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
