using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.TicketsActivos.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsActivos;

public record CrearTicketsActivosCommand(TicketsActivosInputDto Datos) : IRequest<ResponseDto<TicketsActivosResponseDto>>;

internal sealed class CrearTicketsActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearTicketsActivosCommandHandler> _logger) : IRequestHandler<CrearTicketsActivosCommand, ResponseDto<TicketsActivosResponseDto>>
{
    public async Task<ResponseDto<TicketsActivosResponseDto>> Handle(CrearTicketsActivosCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new Dictionary<string, object>() { ["IdTicket"] = input.IdTicket, ["IdActivo"] = input.IdActivo };

            _unitOfWork.Repository<Dictionary<string, object>>("TicketsActivos").Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new TicketsActivosResponseDto() { IdTicket = (Guid)entity["IdTicket"], IdActivo = (Guid)entity["IdActivo"] },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearTicketsActivosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearTicketsActivosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
