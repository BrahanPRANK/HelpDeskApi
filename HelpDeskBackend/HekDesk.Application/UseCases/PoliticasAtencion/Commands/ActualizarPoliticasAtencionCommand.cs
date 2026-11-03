using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PoliticasAtencion;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PoliticasAtencion;

public class ActualizarPoliticasAtencionCommand : IRequest<ResponseDto<PoliticasAtencionResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    [Required]
    [StringLength(10000)]
    public string Prioridad { get; set; } = "media";
    public int MinutosPrimeraRespuesta { get; set; }
    public int MinutosResolucion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class ActualizarPoliticasAtencionCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarPoliticasAtencionCommandHandler> _logger) : IRequestHandler<ActualizarPoliticasAtencionCommand, ResponseDto<PoliticasAtencionResponseDto>>
{
    public async Task<ResponseDto<PoliticasAtencionResponseDto>> Handle(ActualizarPoliticasAtencionCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Prioridad = request.Prioridad;
            entity.MinutosPrimeraRespuesta = request.MinutosPrimeraRespuesta;
            entity.MinutosResolucion = request.MinutosResolucion;
            entity.Activo = request.Activo;

            await _unitOfWork.Complete();
            return new(200, new PoliticasAtencionResponseDto
            {
                IdPolitica = entity.IdPolitica,
                Prioridad = entity.Prioridad,
                MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta,
                MinutosResolucion = entity.MinutosResolucion,
                Activo = entity.Activo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarPoliticasAtencionCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarPoliticasAtencionCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
