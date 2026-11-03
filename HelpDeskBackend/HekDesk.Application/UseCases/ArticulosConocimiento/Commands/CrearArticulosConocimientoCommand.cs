using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.ArticulosConocimiento;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.ArticulosConocimiento;

public class CrearArticulosConocimientoCommand : IRequest<ResponseDto<ArticulosConocimientoResponseDto>>
{
    public Guid IdCategoriaConocimiento { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Contenido { get; set; } = "";
    public bool EsInterno { get; set; }
}

internal sealed class CrearArticulosConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor) : IRequestHandler<CrearArticulosConocimientoCommand, ResponseDto<ArticulosConocimientoResponseDto>>
{
    public async Task<ResponseDto<ArticulosConocimientoResponseDto>> Handle(CrearArticulosConocimientoCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.ArticulosConocimiento()
            {
                IdArticulo = Guid.NewGuid(),
                IdCategoriaConocimiento = request.IdCategoriaConocimiento,
                Titulo = request.Titulo,
                Contenido = request.Contenido,
                EsInterno = request.EsInterno,
                FechaCreacion = DateTime.UtcNow,
                Estado = "borrador"
            };
            entity.IdAutor = actor.Id;
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.ArticulosConocimiento>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<ArticulosConocimientoResponseDto>(201, new ArticulosConocimientoResponseDto
            {
                IdArticulo = entity.IdArticulo,
                IdAutor = entity.IdAutor,
                IdRevisor = entity.IdRevisor,
                IdCategoriaConocimiento = entity.IdCategoriaConocimiento,
                Titulo = entity.Titulo,
                Contenido = entity.Contenido,
                Estado = entity.Estado,
                EsInterno = entity.EsInterno,
                FechaCreacion = entity.FechaCreacion,
                FechaPublicacion = entity.FechaPublicacion
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<ArticulosConocimientoResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<ArticulosConocimientoResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
