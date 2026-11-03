using System.Text.Json.Serialization;
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

public class ActualizarArticulosConocimientoCommand : IRequest<ResponseDto<ArticulosConocimientoResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdCategoriaConocimiento { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Contenido { get; set; } = "";
    public bool EsInterno { get; set; }
}

internal sealed class ActualizarArticulosConocimientoCommandHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ActualizarArticulosConocimientoCommand, ResponseDto<ArticulosConocimientoResponseDto>>
{
    public async Task<ResponseDto<ArticulosConocimientoResponseDto>> Handle(ActualizarArticulosConocimientoCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.ArticulosConocimiento>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);
            Validation.Require(entity.Estado != "publicado", "Despublique o archive antes de editar un articulo publicado.", 409);
            entity.IdCategoriaConocimiento = request.IdCategoriaConocimiento;
            entity.Titulo = request.Titulo;
            entity.Contenido = request.Contenido;
            entity.EsInterno = request.EsInterno;

            await _unitOfWork.Complete();
            return new ResponseDto<ArticulosConocimientoResponseDto>(200, new ArticulosConocimientoResponseDto
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
            }, "Registro actualizado.");
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
