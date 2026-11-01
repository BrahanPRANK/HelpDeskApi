using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.EquiposSoporte.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EquiposSoporte;

public record CrearEquiposSoporteCommand(EquiposSoporteInputDto Datos) : IRequest<ResponseDto<EquiposSoporteResponseDto>>;

internal sealed class CrearEquiposSoporteCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearEquiposSoporteCommandHandler> _logger) : IRequestHandler<CrearEquiposSoporteCommand, ResponseDto<EquiposSoporteResponseDto>>
{
    public async Task<ResponseDto<EquiposSoporteResponseDto>> Handle(CrearEquiposSoporteCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.EquiposSoporte() { IdEquipo = Guid.NewGuid(), Nombre = input.Nombre, Descripcion = input.Descripcion, Activo = input.Activo };

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new EquiposSoporteResponseDto { IdEquipo = entity.IdEquipo, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearEquiposSoporteCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearEquiposSoporteCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
