using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Activos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Activos;

public record CrearActivosCommand(ActivosInputDto Datos) : IRequest<ResponseDto<ActivosResponseDto>>;

internal sealed class CrearActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearActivosCommandHandler> _logger) : IRequestHandler<CrearActivosCommand, ResponseDto<ActivosResponseDto>>
{
    public async Task<ResponseDto<ActivosResponseDto>> Handle(CrearActivosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.Activos() { IdActivo = Guid.NewGuid(), IdSede = input.IdSede, IdArea = input.IdArea, CodigoPatrimonial = input.CodigoPatrimonial, Nombre = input.Nombre, IdTipoActivo = input.IdTipoActivo, NumeroSerie = input.NumeroSerie, Marca = input.Marca, Modelo = input.Modelo, Estado = input.Estado, FechaRegistro = DateTime.UtcNow };
            Validation.Require(entity.Estado!="asignado","Primero registre el activo y despues su asignacion.");
            _unitOfWork.Repository<global::HelpDesk.Domain.Models.Activos>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new ActivosResponseDto { IdActivo = entity.IdActivo, IdSede = entity.IdSede, IdArea = entity.IdArea, CodigoPatrimonial = entity.CodigoPatrimonial, Nombre = entity.Nombre, IdTipoActivo = entity.IdTipoActivo, NumeroSerie = entity.NumeroSerie, Marca = entity.Marca, Modelo = entity.Modelo, Estado = entity.Estado, FechaRegistro = entity.FechaRegistro },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearActivosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearActivosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
