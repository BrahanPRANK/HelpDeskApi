using System.Text.Json.Serialization;
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

public class ActualizarCategoriasConocimientoCommand : IRequest<ResponseDto<CategoriasConocimientoResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class ActualizarCategoriasConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ActualizarCategoriasConocimientoCommand, ResponseDto<CategoriasConocimientoResponseDto>>
{
    public async Task<ResponseDto<CategoriasConocimientoResponseDto>> Handle(ActualizarCategoriasConocimientoCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Nombre = request.Nombre;
            entity.Descripcion = request.Descripcion;
            entity.Activo = request.Activo;

            await _unitOfWork.Complete();
            return new ResponseDto<CategoriasConocimientoResponseDto>(200, new CategoriasConocimientoResponseDto
            {
                IdCategoriaConocimiento = entity.IdCategoriaConocimiento,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            }, "Registro actualizado.");
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
