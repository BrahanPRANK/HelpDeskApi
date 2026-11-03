using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.RecuperacionesClave;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.RecuperacionesClave;

public class ObtenerRecuperacionesClaveQuery : IRequest<ResponseDto<RecuperacionesClaveResponseDto>>
{
    public Guid IdRecuperacion { get; set; }
}

internal sealed class ObtenerRecuperacionesClaveQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerRecuperacionesClaveQuery, ResponseDto<RecuperacionesClaveResponseDto>>
{
    public async Task<ResponseDto<RecuperacionesClaveResponseDto>> Handle(ObtenerRecuperacionesClaveQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.RecuperacionesClave>().GetByIdAsync(request.IdRecuperacion)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<RecuperacionesClaveResponseDto>(200, new RecuperacionesClaveResponseDto
            {
                IdRecuperacion = entity.IdRecuperacion,
                IdUsuario = entity.IdUsuario,
                FechaSolicitud = entity.FechaSolicitud,
                FechaVencimiento = entity.FechaVencimiento,
                FechaUso = entity.FechaUso
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<RecuperacionesClaveResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<RecuperacionesClaveResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
