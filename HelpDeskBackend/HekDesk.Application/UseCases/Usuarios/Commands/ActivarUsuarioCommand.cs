using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios.Commands;

public class ActivarUsuarioCommand : IRequest<ResponseDto<AccionResultadoDto>>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [StringLength(4000)]
    public string Motivo { get; set; } = string.Empty;
}

internal sealed class ActivarUsuarioCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActivarUsuarioCommandHandler> _logger)
    : IRequestHandler<ActivarUsuarioCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(ActivarUsuarioCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r);
            var result = "completado";
            access.Permission("usuarios.administrar");
            var user = await Required<global::HelpDesk.Domain.Models.Usuarios>(r.Id);

            user.Activo = true;
            result = user.Activo ? "activo" : "inactivo";
            await _unitOfWork.Complete();
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActivarUsuarioCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActivarUsuarioCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id) where T : class =>
        await _unitOfWork.Repository<T>().GetByIdAsync([id]) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
