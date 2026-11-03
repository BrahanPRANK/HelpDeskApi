using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.TicketsArticulos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsArticulos;

public record ActualizarTicketsArticulosCommand(object[] Keys, TicketsArticulosInputDto Datos) : IRequest<ResponseDto<TicketsArticulosResponseDto>>;

internal sealed class ActualizarTicketsArticulosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<ActualizarTicketsArticulosCommandHandler> _logger) : IRequestHandler<ActualizarTicketsArticulosCommand, ResponseDto<TicketsArticulosResponseDto>>
{
    public async Task<ResponseDto<TicketsArticulosResponseDto>> Handle(ActualizarTicketsArticulosCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TicketsArticulos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdUsuario = input.IdUsuario;
            entity.IdUsuario = actor.Id;
            await _unitOfWork.Complete();
            return new(200, new TicketsArticulosResponseDto
            {
                IdTicket = entity.IdTicket,
                IdArticulo = entity.IdArticulo,
                IdUsuario = entity.IdUsuario,
                FechaVinculacion = entity.FechaVinculacion
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarTicketsArticulosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarTicketsArticulosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
