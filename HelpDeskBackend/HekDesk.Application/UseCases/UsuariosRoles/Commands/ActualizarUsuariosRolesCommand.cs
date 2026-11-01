using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.UsuariosRoles.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public record ActualizarUsuariosRolesCommand(object[] Keys, UsuariosRolesInputDto Datos) : IRequest<ResponseDto<UsuariosRolesResponseDto>>;

internal sealed class ActualizarUsuariosRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarUsuariosRolesCommandHandler> _logger) : IRequestHandler<ActualizarUsuariosRolesCommand, ResponseDto<UsuariosRolesResponseDto>>
{
    public async Task<ResponseDto<UsuariosRolesResponseDto>> Handle(ActualizarUsuariosRolesCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);


            await Required<global::HelpDesk.Domain.Models.Usuarios>(entity.IdUsuario,ct);await Required<global::HelpDesk.Domain.Models.Roles>(entity.IdRol,ct);
            await _unitOfWork.Complete(ct);
            return new(200,new UsuariosRolesResponseDto { IdUsuario = entity.IdUsuario, IdRol = entity.IdRol, FechaAsignacion = entity.FechaAsignacion },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarUsuariosRolesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarUsuariosRolesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
