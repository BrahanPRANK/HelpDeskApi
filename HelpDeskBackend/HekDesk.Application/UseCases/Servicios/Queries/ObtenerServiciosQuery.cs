using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Servicios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Servicios;

public record ObtenerServiciosQuery(object[] Keys) : IRequest<ResponseDto<ServiciosResponseDto>>;

internal sealed class ObtenerServiciosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerServiciosQuery, ResponseDto<ServiciosResponseDto>>
{
    public async Task<ResponseDto<ServiciosResponseDto>> Handle(ObtenerServiciosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Servicios>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<ServiciosResponseDto>(200, new ServiciosResponseDto
            {
                IdServicio = entity.IdServicio,
                IdEquipoResponsable = entity.IdEquipoResponsable,
                Nombre = entity.Nombre,
                IdCategoriaServicio = entity.IdCategoriaServicio,
                Descripcion = entity.Descripcion,
                RequiereAprobacion = entity.RequiereAprobacion,
                Activo = entity.Activo
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ServiciosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ServiciosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
