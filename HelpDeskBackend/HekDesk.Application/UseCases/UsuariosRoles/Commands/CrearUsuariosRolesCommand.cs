using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public class CrearUsuariosRolesCommand : IRequest<ResponseDto<UsuariosRolesResponseDto>>
{
    public Guid IdUsuario { get; set; }
    public Guid IdRol { get; set; }
}

internal sealed class CrearUsuariosRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearUsuariosRolesCommand, ResponseDto<UsuariosRolesResponseDto>>
{
    public async Task<ResponseDto<UsuariosRolesResponseDto>> Handle(CrearUsuariosRolesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.UsuariosRoles()
            {
                IdUsuario = request.IdUsuario,
                IdRol = request.IdRol,
                FechaAsignacion = DateTime.UtcNow
            };
            await Required<global::HelpDesk.Domain.Models.Usuarios>(entity.IdUsuario);
            await Required<global::HelpDesk.Domain.Models.Roles>(entity.IdRol);
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<UsuariosRolesResponseDto>(201, new UsuariosRolesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdRol = entity.IdRol,
                FechaAsignacion = entity.FechaAsignacion
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<UsuariosRolesResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<UsuariosRolesResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
