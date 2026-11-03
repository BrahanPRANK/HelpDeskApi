using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.AsignacionesActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesActivos;

public record CrearAsignacionesActivosCommand(AsignacionesActivosInputDto Datos) : IRequest<ResponseDto<AsignacionesActivosResponseDto>>;

internal sealed class CrearAsignacionesActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearAsignacionesActivosCommandHandler> _logger) : IRequestHandler<CrearAsignacionesActivosCommand, ResponseDto<AsignacionesActivosResponseDto>>
{
    public async Task<ResponseDto<AsignacionesActivosResponseDto>> Handle(CrearAsignacionesActivosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.AsignacionesActivos()
            {
                IdAsignacion = Guid.NewGuid(),
                IdActivo = input.IdActivo,
                IdUsuarioResponsable = input.IdUsuarioResponsable,
                Observaciones = input.Observaciones,
                FechaAsignacion = DateTime.UtcNow
            };
            var asset = await Required<global::HelpDesk.Domain.Models.Activos>(entity.IdActivo);
            Validation.Require(asset.Estado == "disponible", "El activo no esta disponible.", 409);
            Validation.Require((await Required<global::HelpDesk.Domain.Models.Usuarios>(entity.IdUsuarioResponsable)).Activo, "El responsable esta inactivo.");
            entity.IdUsuarioAsignador = actor.Id;
            asset.Estado = "asignado";
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesActivos>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new AsignacionesActivosResponseDto
            {
                IdAsignacion = entity.IdAsignacion,
                IdActivo = entity.IdActivo,
                IdUsuarioResponsable = entity.IdUsuarioResponsable,
                IdUsuarioAsignador = entity.IdUsuarioAsignador,
                FechaAsignacion = entity.FechaAsignacion,
                FechaDevolucion = entity.FechaDevolucion,
                Observaciones = entity.Observaciones
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearAsignacionesActivosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearAsignacionesActivosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
