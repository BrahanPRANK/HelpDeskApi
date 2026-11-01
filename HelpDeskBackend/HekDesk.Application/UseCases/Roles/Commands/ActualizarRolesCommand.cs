using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Roles.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Roles;

public record ActualizarRolesCommand(object[] Keys, RolesInputDto Datos) : IRequest<ResponseDto<RolesResponseDto>>;

internal sealed class ActualizarRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarRolesCommandHandler> _logger) : IRequestHandler<ActualizarRolesCommand, ResponseDto<RolesResponseDto>>
{
    public async Task<ResponseDto<RolesResponseDto>> Handle(ActualizarRolesCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            if(new[]{"administrador","empleado","tecnico","coordinador","aprobador"}.Contains(entity.Nombre))
            Validation.Require(input.Nombre==entity.Nombre,"No se renombra un rol base.",409);
            entity.Nombre = input.Nombre;
            entity.Descripcion = input.Descripcion;

            await _unitOfWork.Complete(ct);
            return new(200,new RolesResponseDto { IdRol = entity.IdRol, Nombre = entity.Nombre, Descripcion = entity.Descripcion },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarRolesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarRolesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
