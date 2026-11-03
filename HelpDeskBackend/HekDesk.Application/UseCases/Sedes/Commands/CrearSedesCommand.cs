using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Sedes;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Sedes;

public class CrearSedesCommand : IRequest<ResponseDto<SedesResponseDto>>
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class CrearSedesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearSedesCommand, ResponseDto<SedesResponseDto>>
{
    public async Task<ResponseDto<SedesResponseDto>> Handle(CrearSedesCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Sedes()
            {
                IdSede = Guid.NewGuid(),
                Nombre = request.Nombre,
                Direccion = request.Direccion,
                Activo = request.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Sedes>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<SedesResponseDto>(201, new SedesResponseDto
            {
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Direccion = entity.Direccion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<SedesResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<SedesResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
