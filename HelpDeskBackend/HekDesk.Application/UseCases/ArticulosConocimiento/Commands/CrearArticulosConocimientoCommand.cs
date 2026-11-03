using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ArticulosConocimiento;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ArticulosConocimiento;

public record CrearArticulosConocimientoCommand(ArticulosConocimientoInputDto Datos) : IRequest<ResponseDto<ArticulosConocimientoResponseDto>>;

internal sealed class CrearArticulosConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor, ILogger<CrearArticulosConocimientoCommandHandler> _logger) : IRequestHandler<CrearArticulosConocimientoCommand, ResponseDto<ArticulosConocimientoResponseDto>>
{
    public async Task<ResponseDto<ArticulosConocimientoResponseDto>> Handle(CrearArticulosConocimientoCommand request, CancellationToken ct)
    {
        try
        {
            var input = request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.ArticulosConocimiento()
            {
                IdArticulo = Guid.NewGuid(),
                IdCategoriaConocimiento = input.IdCategoriaConocimiento,
                Titulo = input.Titulo,
                Contenido = input.Contenido,
                EsInterno = input.EsInterno,
                FechaCreacion = DateTime.UtcNow,
                Estado = "borrador"
            };
            entity.IdAutor = actor.Id;
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.ArticulosConocimiento>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new(201, new ArticulosConocimientoResponseDto
            {
                IdArticulo = entity.IdArticulo,
                IdAutor = entity.IdAutor,
                IdRevisor = entity.IdRevisor,
                IdCategoriaConocimiento = entity.IdCategoriaConocimiento,
                Titulo = entity.Titulo,
                Contenido = entity.Contenido,
                Estado = entity.Estado,
                EsInterno = entity.EsInterno,
                FechaCreacion = entity.FechaCreacion,
                FechaPublicacion = entity.FechaPublicacion
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en CrearArticulosConocimientoCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar CrearArticulosConocimientoCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
