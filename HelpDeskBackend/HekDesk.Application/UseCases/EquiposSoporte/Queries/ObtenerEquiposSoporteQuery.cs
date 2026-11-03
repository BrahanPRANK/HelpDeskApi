using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.EquiposSoporte;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EquiposSoporte;

public record ObtenerEquiposSoporteQuery(object[] Keys) : IRequest<ResponseDto<EquiposSoporteResponseDto>>;

internal sealed class ObtenerEquiposSoporteQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerEquiposSoporteQuery, ResponseDto<EquiposSoporteResponseDto>>
{
    public async Task<ResponseDto<EquiposSoporteResponseDto>> Handle(ObtenerEquiposSoporteQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.EquiposSoporte>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<EquiposSoporteResponseDto>(200, new EquiposSoporteResponseDto
            {
                IdEquipo = entity.IdEquipo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<EquiposSoporteResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<EquiposSoporteResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
