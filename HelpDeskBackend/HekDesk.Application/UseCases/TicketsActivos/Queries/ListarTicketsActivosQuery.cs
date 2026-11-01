using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.TicketsActivos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsActivos;

public record ListarTicketsActivosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<TicketsActivosResponseDto>>>;

internal sealed class ListarTicketsActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarTicketsActivosQueryHandler> _logger) : IRequestHandler<ListarTicketsActivosQuery, ResponseDto<PaginaDto<TicketsActivosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<TicketsActivosResponseDto>>> Handle(ListarTicketsActivosQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<Dictionary<string, object>>("TicketsActivos").Query());

            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x["IdTicket"]).ThenBy(x => x["IdActivo"]).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new TicketsActivosResponseDto() { IdTicket = (Guid)entity["IdTicket"], IdActivo = (Guid)entity["IdActivo"] }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarTicketsActivosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarTicketsActivosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
