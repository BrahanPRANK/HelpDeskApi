using System.Text.Json.Serialization;
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

public class EliminarProblemasTicketsCommand : IRequest<ResponseDto<OperacionDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];
}

internal sealed class EliminarProblemasTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarProblemasTicketsCommandHandler> _logger) : IRequestHandler<EliminarProblemasTicketsCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarProblemasTicketsCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            _unitOfWork.Repository<Dictionary<string, object>>("ProblemasTickets").Delete(entity);
            await _unitOfWork.Complete();
            return new(200, new(true), "Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarProblemasTicketsCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarProblemasTicketsCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
