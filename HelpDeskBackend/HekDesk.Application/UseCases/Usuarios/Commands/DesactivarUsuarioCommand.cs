using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios.Commands;

public record DesactivarUsuarioCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class DesactivarUsuarioCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<DesactivarUsuarioCommandHandler> _logger)
    : IRequestHandler<DesactivarUsuarioCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(DesactivarUsuarioCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("usuarios.administrar");
            var user=await Required<global::HelpDesk.Domain.Models.Usuarios>(r.Id,ct);
            await EnsureNotLastAdmin(user.IdUsuario,ct);
            user.Activo=false;result=user.Activo?"activo":"inactivo";
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en DesactivarUsuarioCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar DesactivarUsuarioCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
    private async Task EnsureNotLastAdmin(Guid userId,CancellationToken ct)
    {
        var roleIds=_unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().Query().Where(r=>r.Nombre=="administrador").Select(r=>r.IdRol);
        if(!_unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Any(r=>r.IdUsuario==userId && roleIds.Contains(r.IdRol)))return;
        var others=_unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().Query().Where(r=>roleIds.Contains(r.IdRol) && r.IdUsuario!=userId).Select(r=>r.IdUsuario);
        Validation.Require(await _unitOfWork.CountAsync(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().Query().Where(u=>u.Activo && others.Contains(u.IdUsuario)),ct)>0,
            "Debe conservarse al menos un administrador activo.",409);
    }
}
