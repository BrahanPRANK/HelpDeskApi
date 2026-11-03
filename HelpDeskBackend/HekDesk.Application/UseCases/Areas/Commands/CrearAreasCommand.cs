using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Areas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Areas;

public record CrearAreasCommand(AreasInputDto Datos) : IRequest<ResponseDto<AreasResponseDto>>;

internal sealed class CrearAreasCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearAreasCommandHandler> _logger) : IRequestHandler<CrearAreasCommand, ResponseDto<AreasResponseDto>>
{
    public async Task<ResponseDto<AreasResponseDto>> Handle(CrearAreasCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Areas()
            {
                IdArea = Guid.NewGuid(),
                IdSede = input.IdSede,
                Nombre = input.Nombre,
                Activo = input.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Areas>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new AreasResponseDto
            {
                IdArea = entity.IdArea,
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearAreasCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearAreasCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
