using System.Text.Json.Serialization;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
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

internal sealed class EliminarIntegrantesEquipoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<EliminarIntegrantesEquipoCommand, ResponseDto<OperacionDto>>
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
            return new ResponseDto<OperacionDto>(200, new OperacionDto(true), "Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<OperacionDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<OperacionDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
