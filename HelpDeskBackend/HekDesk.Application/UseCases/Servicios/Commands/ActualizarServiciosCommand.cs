using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Servicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Servicios;

public record ActualizarServiciosCommand(object[] Keys, ServiciosInputDto Datos) : IRequest<ResponseDto<ServiciosResponseDto>>;

internal sealed class ActualizarServiciosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarServiciosCommandHandler> _logger) : IRequestHandler<ActualizarServiciosCommand, ResponseDto<ServiciosResponseDto>>
{
    public async Task<ResponseDto<ServiciosResponseDto>> Handle(ActualizarServiciosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Servicios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdEquipoResponsable = input.IdEquipoResponsable;
            entity.Nombre = input.Nombre;
            entity.IdCategoriaServicio = input.IdCategoriaServicio;
            entity.Descripcion = input.Descripcion;
            entity.RequiereAprobacion = input.RequiereAprobacion;
            entity.Activo = input.Activo;

            await _unitOfWork.Complete();
            return new(200, new ServiciosResponseDto
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
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarServiciosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarServiciosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
