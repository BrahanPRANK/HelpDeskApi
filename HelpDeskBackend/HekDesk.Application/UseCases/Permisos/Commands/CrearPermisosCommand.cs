using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Permisos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Permisos;

public record CrearPermisosCommand(PermisosInputDto Datos) : IRequest<ResponseDto<PermisosResponseDto>>;

internal sealed class CrearPermisosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearPermisosCommandHandler> _logger) : IRequestHandler<CrearPermisosCommand, ResponseDto<PermisosResponseDto>>
{
    public async Task<ResponseDto<PermisosResponseDto>> Handle(CrearPermisosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.Permisos() { IdPermiso = Guid.NewGuid(), Codigo = input.Codigo, Modulo = input.Modulo, Descripcion = input.Descripcion };

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new PermisosResponseDto { IdPermiso = entity.IdPermiso, Codigo = entity.Codigo, Modulo = entity.Modulo, Descripcion = entity.Descripcion },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearPermisosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearPermisosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
