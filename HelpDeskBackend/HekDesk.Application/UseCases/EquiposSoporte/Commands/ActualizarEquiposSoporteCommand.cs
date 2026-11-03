using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.EquiposSoporte;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EquiposSoporte;

public class ActualizarEquiposSoporteCommand : IRequest<ResponseDto<EquiposSoporteResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class ActualizarEquiposSoporteCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarEquiposSoporteCommandHandler> _logger) : IRequestHandler<ActualizarEquiposSoporteCommand, ResponseDto<EquiposSoporteResponseDto>>
{
    public async Task<ResponseDto<EquiposSoporteResponseDto>> Handle(ActualizarEquiposSoporteCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Nombre = request.Nombre;
            entity.Descripcion = request.Descripcion;
            entity.Activo = request.Activo;

            await _unitOfWork.Complete();
            return new(200, new EquiposSoporteResponseDto
            {
                IdEquipo = entity.IdEquipo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarEquiposSoporteCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarEquiposSoporteCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
