using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CategoriasConocimiento.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public record CrearCategoriasConocimientoCommand(CategoriasConocimientoInputDto Datos) : IRequest<ResponseDto<CategoriasConocimientoResponseDto>>;

internal sealed class CrearCategoriasConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearCategoriasConocimientoCommandHandler> _logger) : IRequestHandler<CrearCategoriasConocimientoCommand, ResponseDto<CategoriasConocimientoResponseDto>>
{
    public async Task<ResponseDto<CategoriasConocimientoResponseDto>> Handle(CrearCategoriasConocimientoCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.CategoriasConocimiento() { IdCategoriaConocimiento = Guid.NewGuid(), Nombre = input.Nombre, Descripcion = input.Descripcion, Activo = input.Activo };

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new CategoriasConocimientoResponseDto { IdCategoriaConocimiento = entity.IdCategoriaConocimiento, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearCategoriasConocimientoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearCategoriasConocimientoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
