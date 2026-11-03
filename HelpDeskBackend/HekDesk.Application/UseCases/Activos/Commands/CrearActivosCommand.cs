using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Activos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Activos;

public class CrearActivosCommand : IRequest<ResponseDto<ActivosResponseDto>>
{
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

internal sealed class CrearActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearActivosCommandHandler> _logger) : IRequestHandler<CrearActivosCommand, ResponseDto<ActivosResponseDto>>
{
    public async Task<ResponseDto<ActivosResponseDto>> Handle(CrearActivosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Activos()
            {
                IdActivo = Guid.NewGuid(),
                IdSede = request.IdSede,
                IdArea = request.IdArea,
                CodigoPatrimonial = request.CodigoPatrimonial,
                Nombre = request.Nombre,
                IdTipoActivo = request.IdTipoActivo,
                NumeroSerie = request.NumeroSerie,
                Marca = request.Marca,
                Modelo = request.Modelo,
                Estado = request.Estado,
                FechaRegistro = DateTime.UtcNow
            };
            Validation.Require(entity.Estado != "asignado", "Primero registre el activo y despues su asignacion.");
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Activos>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new ActivosResponseDto
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
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearActivosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearActivosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
