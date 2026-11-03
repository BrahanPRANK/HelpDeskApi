using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.AsignacionesActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesActivos;

public record ObtenerAsignacionesActivosQuery(object[] Keys) : IRequest<ResponseDto<AsignacionesActivosResponseDto>>;

internal sealed class ObtenerAsignacionesActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerAsignacionesActivosQuery, ResponseDto<AsignacionesActivosResponseDto>>
{
    public async Task<ResponseDto<AsignacionesActivosResponseDto>> Handle(ObtenerAsignacionesActivosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesActivos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<AsignacionesActivosResponseDto>(200, new AsignacionesActivosResponseDto
            {
                IdAsignacion = entity.IdAsignacion,
                IdActivo = entity.IdActivo,
                IdUsuarioResponsable = entity.IdUsuarioResponsable,
                IdUsuarioAsignador = entity.IdUsuarioAsignador,
                FechaAsignacion = entity.FechaAsignacion,
                FechaDevolucion = entity.FechaDevolucion,
                Observaciones = entity.Observaciones
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AsignacionesActivosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AsignacionesActivosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
