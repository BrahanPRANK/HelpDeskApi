using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TiposActivos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TiposActivos;

public class CrearTiposActivosCommand : IRequest<ResponseDto<TiposActivosResponseDto>>
{
    [Required]
    [StringLength(10000)]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class CrearTiposActivosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearTiposActivosCommand, ResponseDto<TiposActivosResponseDto>>
{
    public async Task<ResponseDto<TiposActivosResponseDto>> Handle(CrearTiposActivosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.TiposActivos()
            {
                IdTipoActivo = Guid.NewGuid(),
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Activo = request.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TiposActivos>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<TiposActivosResponseDto>(201, new TiposActivosResponseDto
            {
                IdTipoActivo = entity.IdTipoActivo,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<TiposActivosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<TiposActivosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
