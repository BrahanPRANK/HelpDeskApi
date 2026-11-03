using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.IntegrantesEquipo;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo;

public class CrearIntegrantesEquipoCommand : IRequest<ResponseDto<IntegrantesEquipoResponseDto>>
{
    public Guid IdEquipo { get; set; }
    public Guid IdUsuario { get; set; }
    public bool EsCoordinador { get; set; }
    public bool Disponible { get; set; } = true;
    public bool Activo { get; set; } = true;
}

internal sealed class CrearIntegrantesEquipoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearIntegrantesEquipoCommand, ResponseDto<IntegrantesEquipoResponseDto>>
{
    public async Task<ResponseDto<IntegrantesEquipoResponseDto>> Handle(CrearIntegrantesEquipoCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.IntegrantesEquipo()
            {
                IdEquipo = request.IdEquipo,
                IdUsuario = request.IdUsuario,
                EsCoordinador = request.EsCoordinador,
                Disponible = request.Disponible,
                Activo = request.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<IntegrantesEquipoResponseDto>(201, new IntegrantesEquipoResponseDto
            {
                IdEquipo = entity.IdEquipo,
                IdUsuario = entity.IdUsuario,
                EsCoordinador = entity.EsCoordinador,
                Disponible = entity.Disponible,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<IntegrantesEquipoResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<IntegrantesEquipoResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
