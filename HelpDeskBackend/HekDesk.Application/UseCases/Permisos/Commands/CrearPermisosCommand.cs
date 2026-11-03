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

public class CrearPermisosCommand : IRequest<ResponseDto<PermisosResponseDto>>
{
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

internal sealed class CrearPermisosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearPermisosCommand, ResponseDto<PermisosResponseDto>>
{
    public async Task<ResponseDto<PermisosResponseDto>> Handle(CrearPermisosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Permisos()
            {
                IdPermiso = Guid.NewGuid(),
                Codigo = request.Codigo,
                Modulo = request.Modulo,
                Descripcion = request.Descripcion
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<PermisosResponseDto>(201, new PermisosResponseDto
            {
                IdPermiso = entity.IdPermiso,
                Codigo = entity.Codigo,
                Modulo = entity.Modulo,
                Descripcion = entity.Descripcion
            }, "Registro creado.");
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
