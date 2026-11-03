using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Encuestas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Encuestas;

public record ObtenerEncuestasQuery(object[] Keys) : IRequest<ResponseDto<EncuestasResponseDto>>;

internal sealed class ObtenerEncuestasQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerEncuestasQuery, ResponseDto<EncuestasResponseDto>>
{
    public async Task<ResponseDto<EncuestasResponseDto>> Handle(ObtenerEncuestasQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Encuestas>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<EncuestasResponseDto>(200, new EncuestasResponseDto
            {
                IdEncuesta = entity.IdEncuesta,
                IdTicket = entity.IdTicket,
                IdUsuario = entity.IdUsuario,
                Calificacion = entity.Calificacion,
                Comentario = entity.Comentario,
                FechaRespuesta = entity.FechaRespuesta
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<EncuestasResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<EncuestasResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
