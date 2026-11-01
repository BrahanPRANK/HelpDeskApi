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

public record EmitirRecuperacionCommand(Guid IdUsuario):IRequest<ResponseDto<TokenRecuperacionDto>>;
internal sealed class EmitirRecuperacionCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, ILogger<EmitirRecuperacionCommandHandler> _logger) : IRequestHandler<EmitirRecuperacionCommand, ResponseDto<TokenRecuperacionDto>>
{
    public async Task<ResponseDto<TokenRecuperacionDto>> Handle(EmitirRecuperacionCommand request, CancellationToken ct)
    {
        try
        {
            var userId=request.IdUsuario;

            Validation.Require(actor.HasPermission("usuarios.administrar"),"Requiere administracion.",403);
            var user=await _unitOfWork.Repository<Usuarios>().FindAsync([userId], ct) ?? throw new BusinessException(404,"Usuario no encontrado.");
            Validation.Require(user.Activo,"Usuario inactivo.");
            var pending=await _unitOfWork.ListAsync(_unitOfWork.Repository<RecuperacionesClave>().Query().Where(x=>x.IdUsuario==userId && x.FechaUso==null),ct);
            foreach(var old in pending)old.FechaUso=DateTime.UtcNow>old.FechaVencimiento?old.FechaVencimiento:DateTime.UtcNow;
            var token=Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var expiry=DateTime.UtcNow.AddMinutes(15);
            _unitOfWork.Repository<RecuperacionesClave>().Add(new RecuperacionesClave {IdRecuperacion=Guid.NewGuid(),IdUsuario=userId,ResumenToken=Digest(token),FechaSolicitud=DateTime.UtcNow,FechaVencimiento=expiry});
            await _unitOfWork.Complete(ct);return new(201,new(token,expiry),"Entregue el token por un canal verificado. No se envio correo.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EmitirRecuperacionCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar EmitirRecuperacionCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }
    private static string Digest(string value)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

}
