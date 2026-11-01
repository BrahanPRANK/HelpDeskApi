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

public record CambiarClaveCommand(CambioClaveDto Datos):IRequest<ResponseDto<OperacionDto>>;
internal sealed class CambiarClaveCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, IPasswordService passwords, ILogger<CambiarClaveCommandHandler> _logger) : IRequestHandler<CambiarClaveCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(CambiarClaveCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;

            Validation.Check(input);
            var user=await _unitOfWork.Repository<Usuarios>().FindAsync([actor.Id], ct) ?? throw new BusinessException(401,"Usuario no disponible.");
            Validation.Require(passwords.Verify(user.ClaveHash,input.ClaveActual),"La clave actual no coincide.",400);
            user.ClaveHash=passwords.Hash(input.ClaveNueva);await _unitOfWork.Complete(ct);
            return new(200,new(true),"Clave actualizada; vuelva a iniciar sesion.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CambiarClaveCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CambiarClaveCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
