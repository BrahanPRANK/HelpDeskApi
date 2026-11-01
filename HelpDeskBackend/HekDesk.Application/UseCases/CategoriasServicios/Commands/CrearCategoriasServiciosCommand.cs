using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CategoriasServicios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasServicios;

public record CrearCategoriasServiciosCommand(CategoriasServiciosInputDto Datos) : IRequest<ResponseDto<CategoriasServiciosResponseDto>>;

internal sealed class CrearCategoriasServiciosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearCategoriasServiciosCommandHandler> _logger) : IRequestHandler<CrearCategoriasServiciosCommand, ResponseDto<CategoriasServiciosResponseDto>>
{
    public async Task<ResponseDto<CategoriasServiciosResponseDto>> Handle(CrearCategoriasServiciosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.CategoriasServicios() { IdCategoriaServicio = Guid.NewGuid(), Nombre = input.Nombre, Descripcion = input.Descripcion, Activo = input.Activo };

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasServicios>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new CategoriasServiciosResponseDto { IdCategoriaServicio = entity.IdCategoriaServicio, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Activo = entity.Activo },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearCategoriasServiciosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearCategoriasServiciosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
