using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.UsuariosRoles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.UsuariosRoles;

public record ObtenerUsuariosRolesQuery(object[] Keys) : IRequest<ResponseDto<UsuariosRolesResponseDto>>;

internal sealed class ObtenerUsuariosRolesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerUsuariosRolesQuery, ResponseDto<UsuariosRolesResponseDto>>
{
    public async Task<ResponseDto<UsuariosRolesResponseDto>> Handle(ObtenerUsuariosRolesQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.UsuariosRoles>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<UsuariosRolesResponseDto>(200, new UsuariosRolesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdRol = entity.IdRol,
                FechaAsignacion = entity.FechaAsignacion
            });
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

}
