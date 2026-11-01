using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ArticulosConocimiento.Commands;

public record PublicarArticuloCommand(Guid Id, AccionDto Datos) : IRequest<ResponseDto<AccionResultadoDto>>;

internal sealed class PublicarArticuloCommandHandler(IUnitOfWork _unitOfWork, ICurrentUser actor, AccessRules access, ILogger<PublicarArticuloCommandHandler> _logger)
    : IRequestHandler<PublicarArticuloCommand, ResponseDto<AccionResultadoDto>>
{
    public async Task<ResponseDto<AccionResultadoDto>> Handle(PublicarArticuloCommand r, CancellationToken ct)
    {
        try
        {
            Validation.Check(r.Datos);
            var result="completado";
            access.Permission("conocimiento.publicar");
            var article=await Required<global::HelpDesk.Domain.Models.ArticulosConocimiento>(r.Id,ct);
            article.Estado="publicado";article.IdRevisor=actor.Id;article.FechaPublicacion=DateTime.UtcNow;
            result=article.Estado;
            await _unitOfWork.Complete(ct);
            return new(200, new(r.Id, result));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en PublicarArticuloCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar PublicarArticuloCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

    private async Task<T> Required<T>(Guid id, CancellationToken ct) where T : class =>
        await _unitOfWork.Repository<T>().FindAsync([id], ct) ?? throw new BusinessException(404, "Registro relacionado no encontrado.");
}
