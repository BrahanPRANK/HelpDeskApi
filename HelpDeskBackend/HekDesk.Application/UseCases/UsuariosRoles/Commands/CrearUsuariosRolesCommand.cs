using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public record CrearUsuariosRolesCommand(UsuariosRolesInputDto Datos) : IRequest<ResponseDto<UsuariosRolesResponseDto>>;

internal sealed class CrearUsuariosRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearUsuariosRolesCommandHandler> _logger) : IRequestHandler<CrearUsuariosRolesCommand, ResponseDto<UsuariosRolesResponseDto>>
{
    public async Task<ResponseDto<UsuariosRolesResponseDto>> Handle(CrearUsuariosRolesCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.UsuariosRoles()
            {
                IdUsuario = input.IdUsuario,
                IdRol = input.IdRol,
                FechaAsignacion = DateTime.UtcNow
            };
            await Required<global::HelpDesk.Domain.Models.Usuarios>(entity.IdUsuario);
            await Required<global::HelpDesk.Domain.Models.Roles>(entity.IdRol);
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new UsuariosRolesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdRol = entity.IdRol,
                FechaAsignacion = entity.FechaAsignacion
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearUsuariosRolesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearUsuariosRolesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
