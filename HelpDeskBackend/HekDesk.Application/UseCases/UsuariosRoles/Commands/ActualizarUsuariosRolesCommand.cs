using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public class ActualizarUsuariosRolesCommand : IRequest<ResponseDto<UsuariosRolesResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdUsuario { get; set; }
    public Guid IdRol { get; set; }
}

internal sealed class ActualizarUsuariosRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarUsuariosRolesCommandHandler> _logger) : IRequestHandler<ActualizarUsuariosRolesCommand, ResponseDto<UsuariosRolesResponseDto>>
{
    public async Task<ResponseDto<UsuariosRolesResponseDto>> Handle(ActualizarUsuariosRolesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            await Required<global::HelpDesk.Domain.Models.Usuarios>(entity.IdUsuario);
            await Required<global::HelpDesk.Domain.Models.Roles>(entity.IdRol);
            await _unitOfWork.Complete();
            return new(200, new UsuariosRolesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdRol = entity.IdRol,
                FechaAsignacion = entity.FechaAsignacion
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarUsuariosRolesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarUsuariosRolesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
