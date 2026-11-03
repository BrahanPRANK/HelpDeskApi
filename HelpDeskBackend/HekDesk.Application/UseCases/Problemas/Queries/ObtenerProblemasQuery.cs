using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Problemas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Problemas;

public record ObtenerProblemasQuery(object[] Keys) : IRequest<ResponseDto<ProblemasResponseDto>>;

internal sealed class ObtenerProblemasQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerProblemasQuery, ResponseDto<ProblemasResponseDto>>
{
    public async Task<ResponseDto<ProblemasResponseDto>> Handle(ObtenerProblemasQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Problemas>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<ProblemasResponseDto>(200, new ProblemasResponseDto
            {
                IdProblema = entity.IdProblema,
                IdResponsable = entity.IdResponsable,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                Estado = entity.Estado,
                CausaRaiz = entity.CausaRaiz,
                SolucionTemporal = entity.SolucionTemporal,
                SolucionDefinitiva = entity.SolucionDefinitiva,
                FechaCreacion = entity.FechaCreacion
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ProblemasResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ProblemasResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
