using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ProblemasTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ProblemasTickets;

public class CrearProblemasTicketsCommand : IRequest<ResponseDto<ProblemasTicketsResponseDto>>
{
    public Guid IdProblema { get; set; }
    public Guid IdTicket { get; set; }
}

internal sealed class CrearProblemasTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<CrearProblemasTicketsCommandHandler> _logger) : IRequestHandler<CrearProblemasTicketsCommand, ResponseDto<ProblemasTicketsResponseDto>>
{
    public async Task<ResponseDto<ProblemasTicketsResponseDto>> Handle(CrearProblemasTicketsCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new Dictionary<string, object>() { ["IdProblema"] = request.IdProblema, ["IdTicket"] = request.IdTicket };

            await _unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new ProblemasTicketsResponseDto()
            {
                IdProblema = (Guid)entity["IdProblema"],
                IdTicket = (Guid)entity["IdTicket"]
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearProblemasTicketsCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearProblemasTicketsCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
