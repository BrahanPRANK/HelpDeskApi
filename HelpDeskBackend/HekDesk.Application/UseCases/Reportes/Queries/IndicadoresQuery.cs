using HelpDesk.Domain.Dtos.Reportes;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Reportes.Queries;

public record IndicadoresQuery() : IRequest<ResponseDto<IndicadoresDto>>;
internal sealed class IndicadoresHandler(AccessRules access) : IRequestHandler<IndicadoresQuery, ResponseDto<IndicadoresDto>>
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
            return new ResponseDto<IndicadoresDto>(200, new IndicadoresDto(total, pending, resolved, closed));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<IndicadoresDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<IndicadoresDto>(500, default, "No se pudo completar la operacion.");
        }
    }
}
