using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Permisos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Permisos;

public class ActualizarPermisosCommand : IRequest<ResponseDto<PermisosResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    [Required]
    [StringLength(10000)]
    public string Codigo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Modulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
}

internal sealed class ActualizarPermisosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ActualizarPermisosCommand, ResponseDto<PermisosResponseDto>>
{
    public async Task<ResponseDto<PermisosResponseDto>> Handle(ActualizarPermisosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Codigo = request.Codigo;
            entity.Modulo = request.Modulo;
            entity.Descripcion = request.Descripcion;

            await _unitOfWork.Complete();
            return new ResponseDto<PermisosResponseDto>(200, new PermisosResponseDto
            {
                IdPermiso = entity.IdPermiso,
                Codigo = entity.Codigo,
                Modulo = entity.Modulo,
                Descripcion = entity.Descripcion
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PermisosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PermisosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
