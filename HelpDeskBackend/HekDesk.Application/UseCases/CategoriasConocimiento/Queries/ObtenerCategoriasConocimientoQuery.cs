using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CategoriasConocimiento;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasConocimiento;

public class ObtenerCategoriasConocimientoQuery : IRequest<ResponseDto<CategoriasConocimientoResponseDto>>
{
    public Guid IdCategoriaConocimiento { get; set; }
}

internal sealed class ObtenerCategoriasConocimientoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerCategoriasConocimientoQuery, ResponseDto<CategoriasConocimientoResponseDto>>
{
    public async Task<ResponseDto<CategoriasConocimientoResponseDto>> Handle(ObtenerCategoriasConocimientoQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasConocimiento>().GetByIdAsync(request.IdCategoriaConocimiento)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<CategoriasConocimientoResponseDto>(200, new CategoriasConocimientoResponseDto
            {
                IdCategoriaConocimiento = entity.IdCategoriaConocimiento,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<CategoriasConocimientoResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<CategoriasConocimientoResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
