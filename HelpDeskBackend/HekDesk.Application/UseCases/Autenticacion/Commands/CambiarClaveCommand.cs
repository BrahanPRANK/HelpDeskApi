using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Domain.Dtos.Autenticacion;
using System.Security.Cryptography;
using System.Text;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.Security;

public class CambiarClaveCommand : IRequest<ResponseDto<OperacionDto>>
{
    [Required]
    public string ClaveActual { get; set; } = string.Empty;
    [Required, StringLength(128, MinimumLength = 10)]
    public string ClaveNueva { get; set; } = string.Empty;
}
internal sealed class CambiarClaveCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, IPasswordService passwords) : IRequestHandler<CambiarClaveCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(CambiarClaveCommand request, CancellationToken ct)
    {
        try
        {


            Validation.Check(request);
            var user = await _unitOfWork.Repository<Usuarios>().GetByIdAsync([actor.Id]) ?? throw new BusinessException(401, "Usuario no disponible.");
            Validation.Require(passwords.Verify(user.ClaveHash, request.ClaveActual), "La clave actual no coincide.", 400);
            user.ClaveHash = passwords.Hash(request.ClaveNueva);
            await _unitOfWork.Complete();
            return new ResponseDto<OperacionDto>(200, new OperacionDto(true), "Clave actualizada; vuelva a iniciar sesion.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<OperacionDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<OperacionDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
