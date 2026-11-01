using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.TicketsArticulos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsArticulos;

public record CrearTicketsArticulosCommand(TicketsArticulosInputDto Datos) : IRequest<ResponseDto<TicketsArticulosResponseDto>>;

internal sealed class CrearTicketsArticulosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearTicketsArticulosCommandHandler> _logger) : IRequestHandler<CrearTicketsArticulosCommand, ResponseDto<TicketsArticulosResponseDto>>
{
    public async Task<ResponseDto<TicketsArticulosResponseDto>> Handle(CrearTicketsArticulosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new global::HelpDesk.Domain.Models.TicketsArticulos() { IdTicket = input.IdTicket, IdArticulo = input.IdArticulo, IdUsuario = input.IdUsuario, FechaVinculacion = DateTime.UtcNow };
            entity.IdUsuario=actor.Id;
            _unitOfWork.Repository<global::HelpDesk.Domain.Models.TicketsArticulos>(null).Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new TicketsArticulosResponseDto { IdTicket = entity.IdTicket, IdArticulo = entity.IdArticulo, IdUsuario = entity.IdUsuario, FechaVinculacion = entity.FechaVinculacion },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearTicketsArticulosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearTicketsArticulosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
