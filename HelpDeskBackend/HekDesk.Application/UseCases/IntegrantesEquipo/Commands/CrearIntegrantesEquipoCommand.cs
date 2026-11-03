using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.IntegrantesEquipo;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo;

public record CrearIntegrantesEquipoCommand(IntegrantesEquipoInputDto Datos) : IRequest<ResponseDto<IntegrantesEquipoResponseDto>>;

internal sealed class CrearIntegrantesEquipoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearIntegrantesEquipoCommandHandler> _logger) : IRequestHandler<CrearIntegrantesEquipoCommand, ResponseDto<IntegrantesEquipoResponseDto>>
{
    public async Task<ResponseDto<IntegrantesEquipoResponseDto>> Handle(CrearIntegrantesEquipoCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.IntegrantesEquipo()
            {
                IdEquipo = input.IdEquipo,
                IdUsuario = input.IdUsuario,
                EsCoordinador = input.EsCoordinador,
                Disponible = input.Disponible,
                Activo = input.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new IntegrantesEquipoResponseDto
            {
                IdEquipo = entity.IdEquipo,
                IdUsuario = entity.IdUsuario,
                EsCoordinador = entity.EsCoordinador,
                Disponible = entity.Disponible,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearIntegrantesEquipoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearIntegrantesEquipoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
