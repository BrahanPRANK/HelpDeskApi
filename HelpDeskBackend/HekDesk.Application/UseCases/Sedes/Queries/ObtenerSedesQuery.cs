using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Sedes.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Sedes;

public record ObtenerSedesQuery(object[] Keys) : IRequest<ResponseDto<SedesResponseDto>>;

internal sealed class ObtenerSedesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerSedesQueryHandler> _logger) : IRequestHandler<ObtenerSedesQuery, ResponseDto<SedesResponseDto>>
{
    public async Task<ResponseDto<SedesResponseDto>> Handle(ObtenerSedesQuery request, CancellationToken ct)
    {
        try
        {
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Sedes>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200,new SedesResponseDto { IdSede = entity.IdSede, Nombre = entity.Nombre, Direccion = entity.Direccion, Activo = entity.Activo });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerSedesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerSedesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
