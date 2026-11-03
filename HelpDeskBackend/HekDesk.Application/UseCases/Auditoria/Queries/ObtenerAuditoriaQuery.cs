using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Auditoria;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Auditoria;

public record ObtenerAuditoriaQuery(object[] Keys) : IRequest<ResponseDto<AuditoriaResponseDto>>;

internal sealed class ObtenerAuditoriaQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerAuditoriaQuery, ResponseDto<AuditoriaResponseDto>>
{
    public async Task<ResponseDto<AuditoriaResponseDto>> Handle(ObtenerAuditoriaQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Auditoria>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<AuditoriaResponseDto>(200, new AuditoriaResponseDto
            {
                IdAuditoria = entity.IdAuditoria,
                IdUsuario = entity.IdUsuario,
                Accion = entity.Accion,
                TablaAfectada = entity.TablaAfectada,
                IdRegistro = entity.IdRegistro,
                Descripcion = entity.Descripcion,
                Fecha = entity.Fecha
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<AuditoriaResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<AuditoriaResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
