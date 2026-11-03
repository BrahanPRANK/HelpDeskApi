using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Aprobaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Aprobaciones;

public record ObtenerAprobacionesQuery(object[] Keys) : IRequest<ResponseDto<AprobacionesResponseDto>>;

internal sealed class ObtenerAprobacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerAprobacionesQuery, ResponseDto<AprobacionesResponseDto>>
{
    public async Task<ResponseDto<AprobacionesResponseDto>> Handle(ObtenerAprobacionesQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Aprobaciones>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<AprobacionesResponseDto>(200, new AprobacionesResponseDto
            {
                IdAprobacion = entity.IdAprobacion,
                IdTicket = entity.IdTicket,
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdAprobador = entity.IdAprobador,
                Orden = entity.Orden,
                Motivo = entity.Motivo,
                Estado = entity.Estado,
                ComentarioDecision = entity.ComentarioDecision,
                FechaSolicitud = entity.FechaSolicitud,
                FechaDecision = entity.FechaDecision
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AprobacionesResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AprobacionesResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
