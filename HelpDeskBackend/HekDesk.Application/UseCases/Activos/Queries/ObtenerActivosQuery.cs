using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Activos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Activos;

public record ObtenerActivosQuery(object[] Keys) : IRequest<ResponseDto<ActivosResponseDto>>;

internal sealed class ObtenerActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ObtenerActivosQuery, ResponseDto<ActivosResponseDto>>
{
    public async Task<ResponseDto<ActivosResponseDto>> Handle(ObtenerActivosQuery request, CancellationToken ct)
    {
        try
        {
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Activos>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            return new ResponseDto<ActivosResponseDto>(200, new ActivosResponseDto
            {
                IdActivo = entity.IdActivo,
                IdSede = entity.IdSede,
                IdArea = entity.IdArea,
                CodigoPatrimonial = entity.CodigoPatrimonial,
                Nombre = entity.Nombre,
                IdTipoActivo = entity.IdTipoActivo,
                NumeroSerie = entity.NumeroSerie,
                Marca = entity.Marca,
                Modelo = entity.Modelo,
                Estado = entity.Estado,
                FechaRegistro = entity.FechaRegistro
            });
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ActivosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ActivosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
