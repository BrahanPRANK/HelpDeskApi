using System.Text.Json.Serialization;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Roles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Roles;

public class EliminarRolesCommand : IRequest<ResponseDto<OperacionDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];
}

internal sealed class EliminarRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarRolesCommandHandler> _logger) : IRequestHandler<EliminarRolesCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarRolesCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(!new[] { "administrador", "empleado", "tecnico", "coordinador", "aprobador" }.Contains(entity.Nombre), "Los roles base se conservan.", 409);
            _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>(null).Delete(entity);
            await _unitOfWork.Complete();
            return new(200, new(true), "Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarRolesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarRolesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
