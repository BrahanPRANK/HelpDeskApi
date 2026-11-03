using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Servicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Servicios;

public class ActualizarServiciosCommand : IRequest<ResponseDto<ServiciosResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdEquipoResponsable { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public Guid IdCategoriaServicio { get; set; }
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
    public bool RequiereAprobacion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class ActualizarServiciosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ActualizarServiciosCommand, ResponseDto<ServiciosResponseDto>>
{
    public async Task<ResponseDto<ServiciosResponseDto>> Handle(ActualizarServiciosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Servicios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdEquipoResponsable = request.IdEquipoResponsable;
            entity.Nombre = request.Nombre;
            entity.IdCategoriaServicio = request.IdCategoriaServicio;
            entity.Descripcion = request.Descripcion;
            entity.RequiereAprobacion = request.RequiereAprobacion;
            entity.Activo = request.Activo;

            await _unitOfWork.Complete();
            return new ResponseDto<ServiciosResponseDto>(200, new ServiciosResponseDto
            {
                IdServicio = entity.IdServicio,
                IdEquipoResponsable = entity.IdEquipoResponsable,
                Nombre = entity.Nombre,
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Descripcion = entity.Descripcion,
                RequiereAprobacion = entity.RequiereAprobacion,
                Activo = entity.Activo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ServiciosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ServiciosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
