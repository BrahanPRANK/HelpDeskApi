using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Aprobaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones;

public record ListarAprobacionesQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<AprobacionesResponseDto>>>;

internal sealed class ListarAprobacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarAprobacionesQueryHandler> _logger) : IRequestHandler<ListarAprobacionesQuery, ResponseDto<PaginaDto<AprobacionesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AprobacionesResponseDto>>> Handle(ListarAprobacionesQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Motivo.Contains(options.Buscar));
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAprobacion).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new AprobacionesResponseDto
            {
                IdAprobacion = entity.IdAprobacion,
                IdTicket = entity.IdTicket,
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdAprobador = entity.IdAprobador,
                Orden = entity.Orden,
                Motivo = entity.Motivo,
                Estado = entity.Estado,
                ComentarioDecision = entity.ComentarioDecision,
                FechaSolicitud = entity.FechaSolicitud,
                FechaDecision = entity.FechaDecision
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarAprobacionesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarAprobacionesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
