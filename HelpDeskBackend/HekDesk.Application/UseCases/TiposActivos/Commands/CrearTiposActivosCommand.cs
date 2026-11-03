using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TiposActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TiposActivos;

public record CrearTiposActivosCommand(TiposActivosInputDto Datos) : IRequest<ResponseDto<TiposActivosResponseDto>>;

internal sealed class CrearTiposActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearTiposActivosCommandHandler> _logger) : IRequestHandler<CrearTiposActivosCommand, ResponseDto<TiposActivosResponseDto>>
{
    public async Task<ResponseDto<TiposActivosResponseDto>> Handle(CrearTiposActivosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.TiposActivos()
            {
                IdTipoActivo = Guid.NewGuid(),
                Nombre = input.Nombre,
                Descripcion = input.Descripcion,
                Activo = input.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TiposActivos>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new TiposActivosResponseDto
            {
                IdTipoActivo = entity.IdTipoActivo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearTiposActivosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearTiposActivosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
