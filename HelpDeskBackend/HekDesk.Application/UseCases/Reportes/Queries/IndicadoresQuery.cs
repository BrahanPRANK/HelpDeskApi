using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Reportes.Queries;
public record IndicadoresDto(int Total,int Pendientes,int Resueltos,int Cerrados);
public record IndicadoresQuery():IRequest<ResponseDto<IndicadoresDto>>;
internal sealed class IndicadoresHandler(IUnitOfWork _unitOfWork,AccessRules access, ILogger<IndicadoresHandler> _logger):IRequestHandler<IndicadoresQuery,ResponseDto<IndicadoresDto>>
{
    public async Task<ResponseDto<IndicadoresDto>> Handle(IndicadoresQuery r,CancellationToken ct)
    {
        try
        {
            var query=access.TicketsScope();
            var total=await _unitOfWork.CountAsync(query,ct);
            var pending=await _unitOfWork.CountAsync(query.Where(x=>x.Estado!="resuelto" && x.Estado!="cerrado" && x.Estado!="cancelado"),ct);
            var resolved=await _unitOfWork.CountAsync(query.Where(x=>x.Estado=="resuelto"),ct);
            var closed=await _unitOfWork.CountAsync(query.Where(x=>x.Estado=="cerrado"),ct);
            return new(200,new(total,pending,resolved,closed));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en IndicadoresHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar IndicadoresHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }
}
