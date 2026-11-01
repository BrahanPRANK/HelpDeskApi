using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.CategoriasConocimiento.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public record EliminarCategoriasConocimientoCommand(object[] Keys) : IRequest<ResponseDto<OperacionDto>>;

internal sealed class EliminarCategoriasConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<EliminarCategoriasConocimientoCommandHandler> _logger) : IRequestHandler<EliminarCategoriasConocimientoCommand, ResponseDto<OperacionDto>>
{
    public async Task<ResponseDto<OperacionDto>> Handle(EliminarCategoriasConocimientoCommand request, CancellationToken ct)
    {
        try
        {
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>(null).Remove(entity);
            await _unitOfWork.Complete(ct);
            return new(200,new(true),"Registro eliminado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en EliminarCategoriasConocimientoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar EliminarCategoriasConocimientoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
