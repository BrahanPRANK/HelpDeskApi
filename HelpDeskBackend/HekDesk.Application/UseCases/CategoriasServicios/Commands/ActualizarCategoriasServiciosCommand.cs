using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.CategoriasServicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasServicios;

public record ActualizarCategoriasServiciosCommand(object[] Keys, CategoriasServiciosInputDto Datos) : IRequest<ResponseDto<CategoriasServiciosResponseDto>>;

internal sealed class ActualizarCategoriasServiciosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarCategoriasServiciosCommandHandler> _logger) : IRequestHandler<ActualizarCategoriasServiciosCommand, ResponseDto<CategoriasServiciosResponseDto>>
{
    public async Task<ResponseDto<CategoriasServiciosResponseDto>> Handle(ActualizarCategoriasServiciosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasServicios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Nombre = input.Nombre;
            entity.Descripcion = input.Descripcion;
            entity.Activo = input.Activo;

            await _unitOfWork.Complete();
            return new(200, new CategoriasServiciosResponseDto
            {
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarCategoriasServiciosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarCategoriasServiciosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
