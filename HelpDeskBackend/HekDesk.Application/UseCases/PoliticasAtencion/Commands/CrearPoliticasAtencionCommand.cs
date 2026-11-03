using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PoliticasAtencion;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PoliticasAtencion;

public class CrearPoliticasAtencionCommand : IRequest<ResponseDto<PoliticasAtencionResponseDto>>
{
    [Required]
    [StringLength(10000)]
    public string Prioridad { get; set; } = "media";
    public int MinutosPrimeraRespuesta { get; set; }
    public int MinutosResolucion { get; set; }
    public bool Activo { get; set; } = true;
}

internal sealed class CrearPoliticasAtencionCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<CrearPoliticasAtencionCommand, ResponseDto<PoliticasAtencionResponseDto>>
{
    public async Task<ResponseDto<PoliticasAtencionResponseDto>> Handle(CrearPoliticasAtencionCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.PoliticasAtencion()
            {
                IdPolitica = Guid.NewGuid(),
                Prioridad = request.Prioridad,
                MinutosPrimeraRespuesta = request.MinutosPrimeraRespuesta,
                MinutosResolucion = request.MinutosResolucion,
                Activo = request.Activo
            };

            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<PoliticasAtencionResponseDto>(201, new PoliticasAtencionResponseDto
            {
                IdPolitica = entity.IdPolitica,
                Prioridad = entity.Prioridad,
                MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta,
                MinutosResolucion = entity.MinutosResolucion,
                Activo = entity.Activo
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PoliticasAtencionResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PoliticasAtencionResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
