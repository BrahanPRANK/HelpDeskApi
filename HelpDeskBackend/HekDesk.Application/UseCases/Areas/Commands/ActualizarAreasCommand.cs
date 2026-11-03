using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Areas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Areas;

public class ActualizarAreasCommand : IRequest<ResponseDto<AreasResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdSede { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public bool Activo { get; set; } = true;
}

internal sealed class ActualizarAreasCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarAreasCommandHandler> _logger) : IRequestHandler<ActualizarAreasCommand, ResponseDto<AreasResponseDto>>
{
    public async Task<ResponseDto<AreasResponseDto>> Handle(ActualizarAreasCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Areas>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdSede = request.IdSede;
            entity.Nombre = request.Nombre;
            entity.Activo = request.Activo;

            await _unitOfWork.Complete();
            return new(200, new AreasResponseDto
            {
                IdArea = entity.IdArea,
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Activo = entity.Activo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarAreasCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarAreasCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
