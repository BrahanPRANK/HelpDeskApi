using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Activos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Activos;

public class ActualizarActivosCommand : IRequest<ResponseDto<ActivosResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdSede { get; set; }
    public Guid? IdArea { get; set; }
    [Required]
    [StringLength(10000)]
    public string CodigoPatrimonial { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public Guid IdTipoActivo { get; set; }
    public string? NumeroSerie { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    [Required]
    [StringLength(10000)]
    public string Estado { get; set; } = "disponible";
}

internal sealed class ActualizarActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ActualizarActivosCommand, ResponseDto<ActivosResponseDto>>
{
    public async Task<ResponseDto<ActivosResponseDto>> Handle(ActualizarActivosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Activos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdSede = request.IdSede;
            entity.IdArea = request.IdArea;
            entity.CodigoPatrimonial = request.CodigoPatrimonial;
            entity.Nombre = request.Nombre;
            entity.IdTipoActivo = request.IdTipoActivo;
            entity.NumeroSerie = request.NumeroSerie;
            entity.Marca = request.Marca;
            entity.Modelo = request.Modelo;
            entity.Estado = request.Estado;
            var assigned = await (_unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesActivos>().Query().Where(x => x.IdActivo == entity.IdActivo && x.FechaDevolucion == null)).CountAsync();
            Validation.Require((assigned > 0) == (entity.Estado == "asignado"), "Cambie la asignacion con asignar/devolver; el estado debe coincidir.", 409);
            await _unitOfWork.Complete();
            return new ResponseDto<ActivosResponseDto>(200, new ActivosResponseDto
            {
                IdActivo = entity.IdActivo,
                IdSede = entity.IdSede,
                IdArea = entity.IdArea,
                CodigoPatrimonial = entity.CodigoPatrimonial,
                Nombre = entity.Nombre,
                IdTipoActivo = entity.IdTipoActivo,
                NumeroSerie = entity.NumeroSerie,
                Marca = entity.Marca,
                Modelo = entity.Modelo,
                Estado = entity.Estado,
                FechaRegistro = entity.FechaRegistro
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ActivosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ActivosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
