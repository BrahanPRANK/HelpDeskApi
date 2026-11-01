using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Activos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Activos;

public record ObtenerActivosQuery(object[] Keys) : IRequest<ResponseDto<ActivosResponseDto>>;

internal sealed class ObtenerActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerActivosQueryHandler> _logger) : IRequestHandler<ObtenerActivosQuery, ResponseDto<ActivosResponseDto>>
{
    public async Task<ResponseDto<ActivosResponseDto>> Handle(ObtenerActivosQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Activos>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new ActivosResponseDto { IdActivo = entity.IdActivo, IdSede = entity.IdSede, IdArea = entity.IdArea, CodigoPatrimonial = entity.CodigoPatrimonial, Nombre = entity.Nombre, IdTipoActivo = entity.IdTipoActivo, NumeroSerie = entity.NumeroSerie, Marca = entity.Marca, Modelo = entity.Modelo, Estado = entity.Estado, FechaRegistro = entity.FechaRegistro });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerActivosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerActivosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
