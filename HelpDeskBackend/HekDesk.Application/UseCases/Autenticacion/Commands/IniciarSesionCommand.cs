using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Domain.Dtos.Autenticacion;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.Security;

public class IniciarSesionCommand : IRequest<ResponseDto<SesionDto>>
{
    [Required, StringLength(100)]
    public string NombreUsuario { get; set; } = string.Empty;
    [Required, StringLength(128)]
    public string Clave { get; set; } = string.Empty;
}
internal sealed class IniciarSesionCommandHandler(IUnitOfWork _unitOfWork, IPasswordService passwords, ITokenService tokens) : IRequestHandler<IniciarSesionCommand, ResponseDto<SesionDto>>
{
    public async Task<ResponseDto<SesionDto>> Handle(IniciarSesionCommand request, CancellationToken ct)
    {
        try
        {


            Validation.Check(request);
            var username = request.NombreUsuario.Trim().ToLowerInvariant();
            var user = (await (_unitOfWork.Repository<Usuarios>().Query().Where(x => x.NombreUsuario.ToLower() == username)).ToListAsync()).SingleOrDefault();
            if (user is null || !user.Activo || !passwords.Verify(user.ClaveHash, request.Clave))
                throw new BusinessException(401, "Credenciales incorrectas.");
            var roleIds = _unitOfWork.Repository<UsuariosRoles>().Query().Where(x => x.IdUsuario == user.IdUsuario).Select(x => x.IdRol);
            var roles = await (_unitOfWork.Repository<Roles>().Query().Where(x => roleIds.Contains(x.IdRol)).Select(x => x.Nombre)).ToListAsync();
            var token = tokens.Create(user.IdUsuario, user.NombreUsuario, roles, Digest(user.ClaveHash));
            return new ResponseDto<SesionDto>(200, new SesionDto(token.Token, token.ExpiresAt, user.IdUsuario, user.NombreUsuario, roles));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<SesionDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<SesionDto>(500, default, "No se pudo completar la operacion.");
        }
    }
    private static string Digest(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

}
