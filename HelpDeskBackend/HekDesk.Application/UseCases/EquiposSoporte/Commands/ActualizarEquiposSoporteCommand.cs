using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.EquiposSoporte.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EquiposSoporte;

public record ActualizarEquiposSoporteCommand(object[] Keys, EquiposSoporteInputDto Datos) : IRequest<ResponseDto<EquiposSoporteResponseDto>>;

internal sealed class ActualizarEquiposSoporteCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarEquiposSoporteCommandHandler> _logger) : IRequestHandler<ActualizarEquiposSoporteCommand, ResponseDto<EquiposSoporteResponseDto>>
{
    public async Task<ResponseDto<EquiposSoporteResponseDto>> Handle(ActualizarEquiposSoporteCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Nombre = input.Nombre;
            entity.Descripcion = input.Descripcion;
            entity.Activo = input.Activo;

            await _unitOfWork.Complete(ct);
            return new(200,new EquiposSoporteResponseDto { IdEquipo = entity.IdEquipo, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarEquiposSoporteCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarEquiposSoporteCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
