using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.CategoriasServicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.CategoriasServicios;

public class ObtenerCategoriasServiciosQuery : IRequest<ResponseDto<CategoriasServiciosResponseDto>>
{
    public Guid IdCategoriaServicio { get; set; }
}

internal sealed class ObtenerCategoriasServiciosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerCategoriasServiciosQuery, ResponseDto<CategoriasServiciosResponseDto>>
{
    public async Task<ResponseDto<CategoriasServiciosResponseDto>> Handle(ObtenerCategoriasServiciosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.CategoriasServicios>().GetByIdAsync(request.IdCategoriaServicio)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<CategoriasServiciosResponseDto>(200, new CategoriasServiciosResponseDto
            {
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<CategoriasServiciosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<CategoriasServiciosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
