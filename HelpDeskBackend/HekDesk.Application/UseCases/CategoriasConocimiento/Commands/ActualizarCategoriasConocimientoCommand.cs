using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CategoriasConocimiento.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public record ActualizarCategoriasConocimientoCommand(object[] Keys, CategoriasConocimientoInputDto Datos) : IRequest<ResponseDto<CategoriasConocimientoResponseDto>>;

internal sealed class ActualizarCategoriasConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarCategoriasConocimientoCommandHandler> _logger) : IRequestHandler<ActualizarCategoriasConocimientoCommand, ResponseDto<CategoriasConocimientoResponseDto>>
{
    public async Task<ResponseDto<CategoriasConocimientoResponseDto>> Handle(ActualizarCategoriasConocimientoCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Nombre = input.Nombre;
            entity.Descripcion = input.Descripcion;
            entity.Activo = input.Activo;

            await _unitOfWork.Complete(ct);
            return new(200,new CategoriasConocimientoResponseDto { IdCategoriaConocimiento = entity.IdCategoriaConocimiento, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarCategoriasConocimientoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarCategoriasConocimientoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
