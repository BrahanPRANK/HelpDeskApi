using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ArticulosConocimiento;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ArticulosConocimiento;

public class ObtenerArticulosConocimientoQuery : IRequest<ResponseDto<ArticulosConocimientoResponseDto>>
{
    public Guid IdArticulo { get; set; }
}

internal sealed class ObtenerArticulosConocimientoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerArticulosConocimientoQuery, ResponseDto<ArticulosConocimientoResponseDto>>
{
    public async Task<ResponseDto<ArticulosConocimientoResponseDto>> Handle(ObtenerArticulosConocimientoQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.ArticulosConocimiento>().GetByIdAsync(request.IdArticulo)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<ArticulosConocimientoResponseDto>(200, new ArticulosConocimientoResponseDto
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
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ArticulosConocimientoResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ArticulosConocimientoResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
