using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios.Commands;

public record ActivarUsuarioCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class ActivarUsuarioCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActivarUsuarioCommandHandler> _logger)
    : IRequestHandler<ActivarUsuarioCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(ActivarUsuarioCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("usuarios.administrar");
            var user=await Required<global::HelpDesk.Domain.Models.Usuarios>(r.Id,ct);

            user.Activo=true;result=user.Activo?"activo":"inactivo";
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActivarUsuarioCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActivarUsuarioCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
