using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Cambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios;

public record ListarCambiosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<CambiosResponseDto>>>;

internal sealed class ListarCambiosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarCambiosQueryHandler> _logger) : IRequestHandler<ListarCambiosQuery, ResponseDto<PaginaDto<CambiosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CambiosResponseDto>>> Handle(ListarCambiosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Cambios>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Titulo.Contains(options.Buscar) || x.Descripcion.Contains(options.Buscar) || x.Riesgo.Contains(options.Buscar));
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdCambio).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new CambiosResponseDto
            {
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdResponsable = entity.IdResponsable,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                Riesgo = entity.Riesgo,
                PlanEjecucion = entity.PlanEjecucion,
                PlanReversion = entity.PlanReversion,
                RequiereAprobacion = entity.RequiereAprobacion,
                Estado = entity.Estado,
                FechaProgramada = entity.FechaProgramada,
                FechaFinalizacion = entity.FechaFinalizacion,
                Resultado = entity.Resultado,
                FechaCreacion = entity.FechaCreacion
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarCambiosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarCambiosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
