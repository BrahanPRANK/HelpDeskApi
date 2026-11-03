using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Permisos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Permisos;

public record ObtenerPermisosQuery(object[] Keys) : IRequest<ResponseDto<PermisosResponseDto>>;

internal sealed class ObtenerPermisosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ObtenerPermisosQueryHandler> _logger) : IRequestHandler<ObtenerPermisosQuery, ResponseDto<PermisosResponseDto>>
{
    public async Task<ResponseDto<PermisosResponseDto>> Handle(ObtenerPermisosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new(200, new PermisosResponseDto
            {
                IdPermiso = entity.IdPermiso,
                Codigo = entity.Codigo,
                Modulo = entity.Modulo,
                Descripcion = entity.Descripcion
            });
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ObtenerPermisosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ObtenerPermisosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
