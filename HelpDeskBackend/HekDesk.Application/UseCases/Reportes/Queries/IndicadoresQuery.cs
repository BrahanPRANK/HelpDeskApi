using HelpDesk.Application.Dtos.Reportes;
using HelpDesk.Application.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Reportes.Queries;

public record IndicadoresQuery() : IRequest<ResponseDto<IndicadoresDto>>;
internal sealed class IndicadoresHandler(AccessRules access, ILogger<IndicadoresHandler> _logger) : IRequestHandler<IndicadoresQuery, ResponseDto<IndicadoresDto>>
{
    public async Task<ResponseDto<IndicadoresDto>> Handle(IndicadoresQuery r, CancellationToken ct)
    {
        try
        {
            var query = access.TicketsScope();
            var total = await (query).CountAsync();
            var pending = await (query.Where(x => x.Estado != "resuelto" && x.Estado != "cerrado" && x.Estado != "cancelado")).CountAsync();
            var resolved = await (query.Where(x => x.Estado == "resuelto")).CountAsync();
            var closed = await (query.Where(x => x.Estado == "cerrado")).CountAsync();
            return new(200, new(total, pending, resolved, closed));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en IndicadoresHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar IndicadoresHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }
}
