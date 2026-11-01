using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CambiosTickets.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CambiosTickets;

public record EliminarCambiosTicketsCommand(object[] Keys) : IRequest<ResponseDto<OperacionDto>>;

internal sealed class EliminarCambiosTicketsCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarCambiosTicketsCommandHandler> _logger) : IRequestHandler<EliminarCambiosTicketsCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarCambiosTicketsCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<Dictionary<string, object>>("CambiosTickets").FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            _unitOfWork.Repository<Dictionary<string, object>>("CambiosTickets").Remove(entity);
            await _unitOfWork.Complete(ct);
            return new(200,new(true),"Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarCambiosTicketsCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarCambiosTicketsCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
