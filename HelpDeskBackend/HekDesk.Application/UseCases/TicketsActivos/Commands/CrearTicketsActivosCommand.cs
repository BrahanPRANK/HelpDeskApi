using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TicketsActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsActivos;

public class CrearTicketsActivosCommand : IRequest<ResponseDto<TicketsActivosResponseDto>>
{
    public Guid IdTicket { get; set; }
    public Guid IdActivo { get; set; }
}

internal sealed class CrearTicketsActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearTicketsActivosCommandHandler> _logger) : IRequestHandler<CrearTicketsActivosCommand, ResponseDto<TicketsActivosResponseDto>>
{
    public async Task<ResponseDto<TicketsActivosResponseDto>> Handle(CrearTicketsActivosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new Dictionary<string, object>() { ["IdTicket"] = request.IdTicket, ["IdActivo"] = request.IdActivo };

            await _unitOfWork.Repository<Dictionary<string, object>>("TicketsActivos").AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new TicketsActivosResponseDto()
            {
                IdTicket = (Guid)entity["IdTicket"],
                IdActivo = (Guid)entity["IdActivo"]
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearTicketsActivosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearTicketsActivosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
