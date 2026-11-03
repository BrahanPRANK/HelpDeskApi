using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Servicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Servicios;

public record CrearServiciosCommand(ServiciosInputDto Datos) : IRequest<ResponseDto<ServiciosResponseDto>>;

internal sealed class CrearServiciosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearServiciosCommandHandler> _logger) : IRequestHandler<CrearServiciosCommand, ResponseDto<ServiciosResponseDto>>
{
    public async Task<ResponseDto<ServiciosResponseDto>> Handle(CrearServiciosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Servicios()
            {
                IdServicio = Guid.NewGuid(),
                IdEquipoResponsable = input.IdEquipoResponsable,
                Nombre = input.Nombre,
                IdCategoriaServicio = input.IdCategoriaServicio,
                Descripcion = input.Descripcion,
                RequiereAprobacion = input.RequiereAprobacion,
                Activo = input.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Servicios>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new ServiciosResponseDto
            {
                IdServicio = entity.IdServicio,
                IdEquipoResponsable = entity.IdEquipoResponsable,
                Nombre = entity.Nombre,
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Descripcion = entity.Descripcion,
                RequiereAprobacion = entity.RequiereAprobacion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearServiciosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearServiciosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
