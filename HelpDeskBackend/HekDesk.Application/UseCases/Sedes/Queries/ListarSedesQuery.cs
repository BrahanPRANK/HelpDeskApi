using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.Sedes;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Sedes;

public record ListarSedesQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<SedesResponseDto>>>;

internal sealed class ListarSedesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarSedesQueryHandler> _logger) : IRequestHandler<ListarSedesQuery, ResponseDto<PaginaDto<SedesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<SedesResponseDto>>> Handle(ListarSedesQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Sedes>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdSede).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new SedesResponseDto
            {
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Direccion = entity.Direccion,
                Activo = entity.Activo
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarSedesQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarSedesQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
