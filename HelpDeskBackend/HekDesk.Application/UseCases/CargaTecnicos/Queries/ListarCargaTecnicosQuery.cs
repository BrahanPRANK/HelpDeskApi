using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CargaTecnicos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CargaTecnicos;

public record ListarCargaTecnicosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<CargaTecnicosResponseDto>>>;

internal sealed class ListarCargaTecnicosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarCargaTecnicosQueryHandler> _logger) : IRequestHandler<ListarCargaTecnicosQuery, ResponseDto<PaginaDto<CargaTecnicosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CargaTecnicosResponseDto>>> Handle(ListarCargaTecnicosQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.CargaTecnicos>().Query());

            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdEquipoAsignado).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new CargaTecnicosResponseDto { IdEquipoAsignado = entity.IdEquipoAsignado, IdTecnicoAsignado = entity.IdTecnicoAsignado, Prioridad = entity.Prioridad, TicketsPendientes = entity.TicketsPendientes }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarCargaTecnicosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarCargaTecnicosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
