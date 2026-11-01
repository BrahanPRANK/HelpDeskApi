using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.ProblemasTickets.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ProblemasTickets;

public record CrearProblemasTicketsCommand(ProblemasTicketsInputDto Datos) : IRequest<ResponseDto<ProblemasTicketsResponseDto>>;

internal sealed class CrearProblemasTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearProblemasTicketsCommandHandler> _logger) : IRequestHandler<CrearProblemasTicketsCommand, ResponseDto<ProblemasTicketsResponseDto>>
{
    public async Task<ResponseDto<ProblemasTicketsResponseDto>> Handle(CrearProblemasTicketsCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=new Dictionary<string, object>() { ["IdProblema"] = input.IdProblema, ["IdTicket"] = input.IdTicket };

            _unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").Add(entity);
            await _unitOfWork.Complete(ct);
            return new(201,new ProblemasTicketsResponseDto() { IdProblema = (Guid)entity["IdProblema"], IdTicket = (Guid)entity["IdTicket"] },"Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearProblemasTicketsCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar CrearProblemasTicketsCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
