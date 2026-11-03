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

public record ActualizarPermisosCommand(object[] Keys, PermisosInputDto Datos) : IRequest<ResponseDto<PermisosResponseDto>>;

internal sealed class ActualizarPermisosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarPermisosCommandHandler> _logger) : IRequestHandler<ActualizarPermisosCommand, ResponseDto<PermisosResponseDto>>
{
    public async Task<ResponseDto<PermisosResponseDto>> Handle(ActualizarPermisosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Permisos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.Codigo = input.Codigo;
            entity.Modulo = input.Modulo;
            entity.Descripcion = input.Descripcion;

            await _unitOfWork.Complete();
            return new(200, new PermisosResponseDto
            {
                IdPermiso = entity.IdPermiso,
                Codigo = entity.Codigo,
                Modulo = entity.Modulo,
                Descripcion = entity.Descripcion
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarPermisosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarPermisosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
