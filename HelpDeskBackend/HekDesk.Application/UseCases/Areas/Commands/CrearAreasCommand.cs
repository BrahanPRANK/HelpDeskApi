using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Areas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Areas;

public class CrearAreasCommand : IRequest<ResponseDto<AreasResponseDto>>
{
    public Guid IdSede { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public bool Activo { get; set; } = true;
}

internal sealed class CrearAreasCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearAreasCommand, ResponseDto<AreasResponseDto>>
{
    public async Task<ResponseDto<AreasResponseDto>> Handle(CrearAreasCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Areas()
            {
                IdArea = Guid.NewGuid(),
                IdSede = request.IdSede,
                Nombre = request.Nombre,
                Activo = request.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Areas>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<AreasResponseDto>(201, new AreasResponseDto
            {
                IdArea = entity.IdArea,
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AreasResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AreasResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
