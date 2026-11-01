using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.ArticulosConocimiento.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ArticulosConocimiento;

public record ActualizarArticulosConocimientoCommand(object[] Keys, ArticulosConocimientoInputDto Datos) : IRequest<ResponseDto<ArticulosConocimientoResponseDto>>;

internal sealed class ActualizarArticulosConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarArticulosConocimientoCommandHandler> _logger) : IRequestHandler<ActualizarArticulosConocimientoCommand, ResponseDto<ArticulosConocimientoResponseDto>>
{
    public async Task<ResponseDto<ArticulosConocimientoResponseDto>> Handle(ActualizarArticulosConocimientoCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.ArticulosConocimiento>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(entity.Estado!="publicado","Despublique o archive antes de editar un articulo publicado.",409);
            entity.IdCategoriaConocimiento = input.IdCategoriaConocimiento;
            entity.Titulo = input.Titulo;
            entity.Contenido = input.Contenido;
            entity.EsInterno = input.EsInterno;

            await _unitOfWork.Complete(ct);
            return new(200,new ArticulosConocimientoResponseDto { IdArticulo = entity.IdArticulo, IdAutor = entity.IdAutor, IdRevisor = entity.IdRevisor, IdCategoriaConocimiento = entity.IdCategoriaConocimiento, Titulo = entity.Titulo, Contenido = entity.Contenido, Estado = entity.Estado, EsInterno = entity.EsInterno, FechaCreacion = entity.FechaCreacion, FechaPublicacion = entity.FechaPublicacion },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarArticulosConocimientoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarArticulosConocimientoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
