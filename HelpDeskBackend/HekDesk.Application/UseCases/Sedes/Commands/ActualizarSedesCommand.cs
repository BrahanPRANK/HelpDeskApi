using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Sedes.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Sedes;

public record ActualizarSedesCommand(object[] Keys, SedesInputDto Datos) : IRequest<ResponseDto<SedesResponseDto>>;

internal sealed class ActualizarSedesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarSedesCommandHandler> _logger) : IRequestHandler<ActualizarSedesCommand, ResponseDto<SedesResponseDto>>
{
    public async Task<ResponseDto<SedesResponseDto>> Handle(ActualizarSedesCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Sedes>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Nombre = input.Nombre;
            entity.Direccion = input.Direccion;
            entity.Activo = input.Activo;

            await _unitOfWork.Complete(ct);
            return new(200,new SedesResponseDto { IdSede = entity.IdSede, Nombre = entity.Nombre, Direccion = entity.Direccion, Activo = entity.Activo },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarSedesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarSedesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
