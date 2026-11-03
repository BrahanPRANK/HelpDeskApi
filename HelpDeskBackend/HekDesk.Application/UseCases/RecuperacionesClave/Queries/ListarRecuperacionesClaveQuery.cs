using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.RecuperacionesClave;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RecuperacionesClave;

public record ListarRecuperacionesClaveQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>>;

internal sealed class ListarRecuperacionesClaveQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarRecuperacionesClaveQueryHandler> _logger) : IRequestHandler<ListarRecuperacionesClaveQuery, ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<RecuperacionesClaveResponseDto>>> Handle(ListarRecuperacionesClaveQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.RecuperacionesClave>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdRecuperacion).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new RecuperacionesClaveResponseDto
            {
                IdRecuperacion = entity.IdRecuperacion,
                IdUsuario = entity.IdUsuario,
                FechaSolicitud = entity.FechaSolicitud,
                FechaVencimiento = entity.FechaVencimiento,
                FechaUso = entity.FechaUso
            })).ToListAsync();
            return new(200, new(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarRecuperacionesClaveQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ListarRecuperacionesClaveQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
