using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.IntegrantesEquipo.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo;

public record ActualizarIntegrantesEquipoCommand(object[] Keys, IntegrantesEquipoInputDto Datos) : IRequest<ResponseDto<IntegrantesEquipoResponseDto>>;

internal sealed class ActualizarIntegrantesEquipoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarIntegrantesEquipoCommandHandler> _logger) : IRequestHandler<ActualizarIntegrantesEquipoCommand, ResponseDto<IntegrantesEquipoResponseDto>>
{
    public async Task<ResponseDto<IntegrantesEquipoResponseDto>> Handle(ActualizarIntegrantesEquipoCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.EsCoordinador = input.EsCoordinador;
            entity.Disponible = input.Disponible;
            entity.Activo = input.Activo;

            await _unitOfWork.Complete(ct);
            return new(200,new IntegrantesEquipoResponseDto { IdEquipo = entity.IdEquipo, IdUsuario = entity.IdUsuario, EsCoordinador = entity.EsCoordinador, Disponible = entity.Disponible, Activo = entity.Activo },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarIntegrantesEquipoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarIntegrantesEquipoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
