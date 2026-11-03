using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Sedes;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Sedes;

public record CrearSedesCommand(SedesInputDto Datos) : IRequest<ResponseDto<SedesResponseDto>>;

internal sealed class CrearSedesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearSedesCommandHandler> _logger) : IRequestHandler<CrearSedesCommand, ResponseDto<SedesResponseDto>>
{
    public async Task<ResponseDto<SedesResponseDto>> Handle(CrearSedesCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Sedes()
            {
                IdSede = Guid.NewGuid(),
                Nombre = input.Nombre,
                Direccion = input.Direccion,
                Activo = input.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Sedes>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new SedesResponseDto
            {
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Direccion = entity.Direccion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearSedesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearSedesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
