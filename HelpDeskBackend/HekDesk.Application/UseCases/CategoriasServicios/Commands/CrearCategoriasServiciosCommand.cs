using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CategoriasServicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasServicios;

public class CrearCategoriasServiciosCommand : IRequest<ResponseDto<CategoriasServiciosResponseDto>>
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class CrearCategoriasServiciosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearCategoriasServiciosCommandHandler> _logger) : IRequestHandler<CrearCategoriasServiciosCommand, ResponseDto<CategoriasServiciosResponseDto>>
{
    public async Task<ResponseDto<CategoriasServiciosResponseDto>> Handle(CrearCategoriasServiciosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.CategoriasServicios()
            {
                IdCategoriaServicio = Guid.NewGuid(),
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasServicios>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new CategoriasServiciosResponseDto
            {
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearCategoriasServiciosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearCategoriasServiciosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
