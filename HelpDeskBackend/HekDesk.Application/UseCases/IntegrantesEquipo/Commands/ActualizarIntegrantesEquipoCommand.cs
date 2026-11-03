using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.IntegrantesEquipo;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo;

public class ActualizarIntegrantesEquipoCommand : IRequest<ResponseDto<IntegrantesEquipoResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdEquipo { get; set; }
    public Guid IdUsuario { get; set; }
    public bool EsCoordinador { get; set; }
    public bool Disponible { get; set; } = true;
    public bool Activo { get; set; } = true;
}

internal sealed class ActualizarIntegrantesEquipoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarIntegrantesEquipoCommandHandler> _logger) : IRequestHandler<ActualizarIntegrantesEquipoCommand, ResponseDto<IntegrantesEquipoResponseDto>>
{
    public async Task<ResponseDto<IntegrantesEquipoResponseDto>> Handle(ActualizarIntegrantesEquipoCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.EsCoordinador = request.EsCoordinador;
            entity.Disponible = request.Disponible;
            entity.Activo = request.Activo;

            await _unitOfWork.Complete();
            return new(200, new IntegrantesEquipoResponseDto
            {
                IdEquipo = entity.IdEquipo,
                IdUsuario = entity.IdUsuario,
                EsCoordinador = entity.EsCoordinador,
                Disponible = entity.Disponible,
                Activo = entity.Activo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarIntegrantesEquipoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarIntegrantesEquipoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
