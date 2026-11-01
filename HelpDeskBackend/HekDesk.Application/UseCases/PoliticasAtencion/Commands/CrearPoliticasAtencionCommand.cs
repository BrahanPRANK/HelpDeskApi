using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.PoliticasAtencion.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PoliticasAtencion;

public record CrearPoliticasAtencionCommand(PoliticasAtencionInputDto Datos) : IRequest<ResponseDto<PoliticasAtencionResponseDto>>;

internal sealed class CrearPoliticasAtencionCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearPoliticasAtencionCommandHandler> _logger) : IRequestHandler<CrearPoliticasAtencionCommand, ResponseDto<PoliticasAtencionResponseDto>>
{
    public async Task<ResponseDto<PoliticasAtencionResponseDto>> Handle(CrearPoliticasAtencionCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.PoliticasAtencion() { IdPolitica = Guid.NewGuid(), Prioridad = input.Prioridad, MinutosPrimeraRespuesta = input.MinutosPrimeraRespuesta, MinutosResolucion = input.MinutosResolucion, Activo = input.Activo };

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new PoliticasAtencionResponseDto { IdPolitica = entity.IdPolitica, Prioridad = entity.Prioridad, MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta, MinutosResolucion = entity.MinutosResolucion, Activo = entity.Activo },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearPoliticasAtencionCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearPoliticasAtencionCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
