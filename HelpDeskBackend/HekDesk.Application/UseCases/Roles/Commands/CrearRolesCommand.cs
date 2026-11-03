using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Roles;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Roles;

public record CrearRolesCommand(RolesInputDto Datos) : IRequest<ResponseDto<RolesResponseDto>>;

internal sealed class CrearRolesCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearRolesCommandHandler> _logger) : IRequestHandler<CrearRolesCommand, ResponseDto<RolesResponseDto>>
{
    public async Task<ResponseDto<RolesResponseDto>> Handle(CrearRolesCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.Roles()
            {
                IdRol = Guid.NewGuid(),
                Nombre = input.Nombre,
                Descripcion = input.Descripcion
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Roles>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new RolesResponseDto
            {
                IdRol = entity.IdRol,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearRolesCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearRolesCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
