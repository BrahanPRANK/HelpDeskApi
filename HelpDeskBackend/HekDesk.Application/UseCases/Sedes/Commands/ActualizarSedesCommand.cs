using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Sedes;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Sedes;

public class ActualizarSedesCommand : IRequest<ResponseDto<SedesResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class ActualizarSedesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarSedesCommandHandler> _logger) : IRequestHandler<ActualizarSedesCommand, ResponseDto<SedesResponseDto>>
{
    public async Task<ResponseDto<SedesResponseDto>> Handle(ActualizarSedesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Sedes>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Nombre = request.Nombre;
            entity.Direccion = request.Direccion;
            entity.Activo = request.Activo;

            await _unitOfWork.Complete();
            return new(200, new SedesResponseDto
            {
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Direccion = entity.Direccion,
                Activo = entity.Activo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarSedesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarSedesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
