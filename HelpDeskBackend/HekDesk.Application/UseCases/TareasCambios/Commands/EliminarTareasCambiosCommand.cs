using HelpDesk.Application.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.Dtos.TareasCambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TareasCambios;

public record EliminarTareasCambiosCommand(object[] Keys) : IRequest<ResponseDto<OperacionDto>>;

internal sealed class EliminarTareasCambiosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarTareasCambiosCommandHandler> _logger) : IRequestHandler<EliminarTareasCambiosCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarTareasCambiosCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TareasCambios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.TareasCambios>(null).Delete(entity);
            await _unitOfWork.Complete();
            return new(200, new(true), "Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarTareasCambiosCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarTareasCambiosCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
