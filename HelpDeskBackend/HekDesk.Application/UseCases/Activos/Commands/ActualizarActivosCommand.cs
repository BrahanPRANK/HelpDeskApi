using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Activos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Activos;

public record ActualizarActivosCommand(object[] Keys, ActivosInputDto Datos) : IRequest<ResponseDto<ActivosResponseDto>>;

internal sealed class ActualizarActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarActivosCommandHandler> _logger) : IRequestHandler<ActualizarActivosCommand, ResponseDto<ActivosResponseDto>>
{
    public async Task<ResponseDto<ActivosResponseDto>> Handle(ActualizarActivosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Activos>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdSede = input.IdSede;
            entity.IdArea = input.IdArea;
            entity.CodigoPatrimonial = input.CodigoPatrimonial;
            entity.Nombre = input.Nombre;
            entity.IdTipoActivo = input.IdTipoActivo;
            entity.NumeroSerie = input.NumeroSerie;
            entity.Marca = input.Marca;
            entity.Modelo = input.Modelo;
            entity.Estado = input.Estado;
            var assigned=await _unitOfWork.CountAsync(_unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesActivos>().Query().Where(x=>x.IdActivo==entity.IdActivo && x.FechaDevolucion==null),ct);
            Validation.Require((assigned>0)==(entity.Estado=="asignado"),"Cambie la asignacion con asignar/devolver; el estado debe coincidir.",409);
            await _unitOfWork.Complete(ct);
            return new(200,new ActivosResponseDto { IdActivo = entity.IdActivo, IdSede = entity.IdSede, IdArea = entity.IdArea, CodigoPatrimonial = entity.CodigoPatrimonial, Nombre = entity.Nombre, IdTipoActivo = entity.IdTipoActivo, NumeroSerie = entity.NumeroSerie, Marca = entity.Marca, Modelo = entity.Modelo, Estado = entity.Estado, FechaRegistro = entity.FechaRegistro },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarActivosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarActivosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
