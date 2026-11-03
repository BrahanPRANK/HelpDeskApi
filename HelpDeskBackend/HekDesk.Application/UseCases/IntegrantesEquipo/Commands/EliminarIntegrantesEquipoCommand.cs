using System.Text.Json.Serialization;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.IntegrantesEquipo;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo;

public class EliminarIntegrantesEquipoCommand : IRequest<ResponseDto<OperacionDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];
}

internal sealed class EliminarIntegrantesEquipoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarIntegrantesEquipoCommandHandler> _logger) : IRequestHandler<EliminarIntegrantesEquipoCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarIntegrantesEquipoCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(await (_unitOfWork.Repository<global::HelpDesk.Domain.Models.Tickets>().Query().Where(t => t.IdEquipoAsignado == entity.IdEquipo && t.IdTecnicoAsignado == entity.IdUsuario)).CountAsync() == 0,
            "El integrante tiene tickets asociados; desactivelo en vez de borrarlo.", 409);
            _unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>(null).Delete(entity);
            await _unitOfWork.Complete();
            return new(200, new(true), "Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarIntegrantesEquipoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarIntegrantesEquipoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
