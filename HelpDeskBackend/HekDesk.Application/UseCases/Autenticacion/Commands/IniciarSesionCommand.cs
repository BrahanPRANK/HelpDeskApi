using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.Security;

public record IniciarSesionCommand(LoginDto Datos):IRequest<ResponseDto<SesionDto>>;
internal sealed class IniciarSesionCommandHandler(IUnitOfWork _unitOfWork, IPasswordService passwords, ITokenService tokens, ILogger<IniciarSesionCommandHandler> _logger) : IRequestHandler<IniciarSesionCommand, ResponseDto<SesionDto>>
{
    public async Task<ResponseDto<SesionDto>> Handle(IniciarSesionCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;

            Validation.Check(input);
            var username=input.NombreUsuario.Trim().ToLowerInvariant();
            var user=(await _unitOfWork.ListAsync(_unitOfWork.Repository<Usuarios>().Query().Where(x=>x.NombreUsuario.ToLower()==username),ct)).SingleOrDefault();
            if(user is null || !user.Activo || !passwords.Verify(user.ClaveHash,input.Clave))
            throw new BusinessException(401,"Credenciales incorrectas.");
            var roleIds=_unitOfWork.Repository<UsuariosRoles>().Query().Where(x=>x.IdUsuario==user.IdUsuario).Select(x=>x.IdRol);
            var roles=await _unitOfWork.ListAsync(_unitOfWork.Repository<Roles>().Query().Where(x=>roleIds.Contains(x.IdRol)).Select(x=>x.Nombre),ct);
            var token=tokens.Create(user.IdUsuario,user.NombreUsuario,roles,Digest(user.ClaveHash));
            return new(200,new(token.Token,token.ExpiresAt,user.IdUsuario,user.NombreUsuario,roles));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en IniciarSesionCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar IniciarSesionCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }
    private static string Digest(string value)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

}
