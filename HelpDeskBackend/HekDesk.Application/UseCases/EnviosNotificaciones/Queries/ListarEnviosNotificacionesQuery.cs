using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.EnviosNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EnviosNotificaciones;

public record ListarEnviosNotificacionesQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>>;

internal sealed class ListarEnviosNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarEnviosNotificacionesQueryHandler> _logger) : IRequestHandler<ListarEnviosNotificacionesQuery, ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>> Handle(ListarEnviosNotificacionesQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.EnviosNotificaciones>().Query());
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEnvio).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new EnviosNotificacionesResponseDto
            {
                IdEnvio = entity.IdEnvio,
                IdNotificacion = entity.IdNotificacion,
                CorreoDestino = entity.CorreoDestino,
                Estado = entity.Estado,
                Intentos = entity.Intentos,
                MaximoIntentos = entity.MaximoIntentos,
                FechaCreacion = entity.FechaCreacion,
                FechaProximoIntento = entity.FechaProximoIntento,
                BloqueoHasta = entity.BloqueoHasta,
                FechaEnvio = entity.FechaEnvio,
                IdentificadorProveedor = entity.IdentificadorProveedor
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarEnviosNotificacionesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarEnviosNotificacionesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
