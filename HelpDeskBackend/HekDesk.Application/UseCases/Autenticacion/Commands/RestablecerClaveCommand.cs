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

public record RestablecerClaveCommand(RestablecerClaveDto Datos):IRequest<ResponseDto<OperacionDto>>;
internal sealed class RestablecerClaveCommandHandler(IUnitOfWork _unitOfWork, IPasswordService passwords, ILogger<RestablecerClaveCommandHandler> _logger) : IRequestHandler<RestablecerClaveCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(RestablecerClaveCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;

            Validation.Check(input);var digest=Digest(input.Token);
            var recovery=(await _unitOfWork.ListAsync(_unitOfWork.Repository<RecuperacionesClave>().Query().Where(x=>x.ResumenToken==digest),ct)).SingleOrDefault();
            Validation.Require(recovery is not null && recovery.FechaUso is null && recovery.FechaVencimiento>DateTime.UtcNow,"Token invalido o vencido.");
            var user=await _unitOfWork.Repository<Usuarios>().FindAsync([recovery!.IdUsuario], ct) ?? throw new BusinessException(400,"Token invalido.");
            Validation.Require(user.Activo,"Token invalido.");
            user.ClaveHash=passwords.Hash(input.ClaveNueva);recovery.FechaUso=DateTime.UtcNow;
            await _unitOfWork.Complete(ct);return new(200,new(true),"Clave restablecida.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en RestablecerClaveCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar RestablecerClaveCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }
    private static string Digest(string value)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

}
