using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CategoriasConocimiento;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public class CrearCategoriasConocimientoCommand : IRequest<ResponseDto<CategoriasConocimientoResponseDto>>
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class CrearCategoriasConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearCategoriasConocimientoCommand, ResponseDto<CategoriasConocimientoResponseDto>>
{
    public async Task<ResponseDto<CategoriasConocimientoResponseDto>> Handle(CrearCategoriasConocimientoCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.CategoriasConocimiento()
            {
                IdCategoriaConocimiento = Guid.NewGuid(),
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<CategoriasConocimientoResponseDto>(201, new CategoriasConocimientoResponseDto
            {
                IdCategoriaConocimiento = entity.IdCategoriaConocimiento,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<CategoriasConocimientoResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<CategoriasConocimientoResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
