using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.RolesPermisos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RolesPermisos;

public class CrearRolesPermisosCommand : IRequest<ResponseDto<RolesPermisosResponseDto>>
{
    public Guid IdRol { get; set; }
    public Guid IdPermiso { get; set; }
}

internal sealed class CrearRolesPermisosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearRolesPermisosCommand, ResponseDto<RolesPermisosResponseDto>>
{
    public async Task<ResponseDto<RolesPermisosResponseDto>> Handle(CrearRolesPermisosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new Dictionary<string, object>() { ["IdRol"] = request.IdRol, ["IdPermiso"] = request.IdPermiso };

            await _unitOfWork.Repository<Dictionary<string, object>>("RolesPermisos").AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<RolesPermisosResponseDto>(201, new RolesPermisosResponseDto()
            {
                IdRol = (Guid)entity["IdRol"],
                IdPermiso = (Guid)entity["IdPermiso"]
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<RolesPermisosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<RolesPermisosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
