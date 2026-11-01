using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ArticulosConocimiento.Commands;

public record ArchivarArticuloCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class ArchivarArticuloCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ArchivarArticuloCommandHandler> _logger)
    : IRequestHandler<ArchivarArticuloCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(ArchivarArticuloCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("conocimiento.publicar");
            var article=await Required<global::HelpDesk.Domain.Models.ArticulosConocimiento>(r.Id,ct);
            article.Estado="archivado";
            result=article.Estado;
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ArchivarArticuloCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ArchivarArticuloCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
