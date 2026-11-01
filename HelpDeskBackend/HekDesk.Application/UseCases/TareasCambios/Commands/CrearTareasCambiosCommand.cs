using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.TareasCambios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TareasCambios;

public record CrearTareasCambiosCommand(TareasCambiosInputDto Datos) : IRequest<ResponseDto<TareasCambiosResponseDto>>;

internal sealed class CrearTareasCambiosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearTareasCambiosCommandHandler> _logger) : IRequestHandler<CrearTareasCambiosCommand, ResponseDto<TareasCambiosResponseDto>>
{
    public async Task<ResponseDto<TareasCambiosResponseDto>> Handle(CrearTareasCambiosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.TareasCambios() { IdTareaCambio = Guid.NewGuid(), IdCambio = input.IdCambio, IdResponsable = input.IdResponsable, Orden = input.Orden, Titulo = input.Titulo, Descripcion = input.Descripcion, Estado = input.Estado, FechaCreacion = DateTime.UtcNow };
            access.EnsureRead(await Required<global::HelpDesk.Domain.Models.Cambios>(entity.IdCambio,ct));
            entity.FechaFinalizacion=entity.Estado=="completada"?DateTime.UtcNow:null;
            _unitOfWork.Repository<global::HelpDesk.Domain.Models.TareasCambios>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new TareasCambiosResponseDto { IdTareaCambio = entity.IdTareaCambio, IdCambio = entity.IdCambio, IdResponsable = entity.IdResponsable, Orden = entity.Orden, Titulo = entity.Titulo, Descripcion = entity.Descripcion, Estado = entity.Estado, FechaCreacion = entity.FechaCreacion, FechaFinalizacion = entity.FechaFinalizacion },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearTareasCambiosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearTareasCambiosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");

}
