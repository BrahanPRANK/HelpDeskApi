using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.PoliticasAtencion;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PoliticasAtencion;

public record ListarPoliticasAtencionQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>>;

internal sealed class ListarPoliticasAtencionQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarPoliticasAtencionQueryHandler> _logger) : IRequestHandler<ListarPoliticasAtencionQuery, ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>> Handle(ListarPoliticasAtencionQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdPolitica).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new PoliticasAtencionResponseDto
            {
                IdPolitica = entity.IdPolitica,
                Prioridad = entity.Prioridad,
                MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta,
                MinutosResolucion = entity.MinutosResolucion,
                Activo = entity.Activo
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarPoliticasAtencionQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarPoliticasAtencionQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
