using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CambiosTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CambiosTickets;

public record CrearCambiosTicketsCommand(CambiosTicketsInputDto Datos) : IRequest<ResponseDto<CambiosTicketsResponseDto>>;

internal sealed class CrearCambiosTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearCambiosTicketsCommandHandler> _logger) : IRequestHandler<CrearCambiosTicketsCommand, ResponseDto<CambiosTicketsResponseDto>>
{
    public async Task<ResponseDto<CambiosTicketsResponseDto>> Handle(CrearCambiosTicketsCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new Dictionary<string, object>() { ["IdCambio"] = input.IdCambio, ["IdTicket"] = input.IdTicket };

            await _unitOfWork.Repository<Dictionary<string, object>>("CambiosTickets").AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new CambiosTicketsResponseDto()
            {
                IdCambio = (Guid)entity["IdCambio"],
                IdTicket = (Guid)entity["IdTicket"]
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearCambiosTicketsCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearCambiosTicketsCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
