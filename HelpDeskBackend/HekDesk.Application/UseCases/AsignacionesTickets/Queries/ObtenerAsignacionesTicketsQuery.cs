using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.AsignacionesTickets;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.AsignacionesTickets;

public record ObtenerAsignacionesTicketsQuery(object[] Keys) : IRequest<ResponseDto<AsignacionesTicketsResponseDto>>;

internal sealed class ObtenerAsignacionesTicketsQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerAsignacionesTicketsQuery, ResponseDto<AsignacionesTicketsResponseDto>>
{
    public async Task<ResponseDto<AsignacionesTicketsResponseDto>> Handle(ObtenerAsignacionesTicketsQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.AsignacionesTickets>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<AsignacionesTicketsResponseDto>(200, new AsignacionesTicketsResponseDto
            {
                IdAsignacionTicket = entity.IdAsignacionTicket,
                IdTicket = entity.IdTicket,
                IdEquipo = entity.IdEquipo,
                IdTecnico = entity.IdTecnico,
                IdAsignador = entity.IdAsignador,
                FechaAsignacion = entity.FechaAsignacion,
                FechaFin = entity.FechaFin,
                Motivo = entity.Motivo
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AsignacionesTicketsResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AsignacionesTicketsResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
