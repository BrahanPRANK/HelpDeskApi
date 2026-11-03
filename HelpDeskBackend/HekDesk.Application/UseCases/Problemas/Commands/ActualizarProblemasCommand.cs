using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Problemas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Problemas;

public class ActualizarProblemasCommand : IRequest<ResponseDto<ProblemasResponseDto>>
{
    [JsonIgnore]
    public object[] Keys { get; set; } = [];

    public Guid IdResponsable { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Estado { get; set; } = "abierto";
    public string? CausaRaiz { get; set; }
    public string? SolucionTemporal { get; set; }
    public string? SolucionDefinitiva { get; set; }
}

internal sealed class ActualizarProblemasCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarProblemasCommandHandler> _logger) : IRequestHandler<ActualizarProblemasCommand, ResponseDto<ProblemasResponseDto>>
{
    public async Task<ResponseDto<ProblemasResponseDto>> Handle(ActualizarProblemasCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Problemas>().GetByIdAsync(request.Keys)
            ?? throw new BusinessException(404, "Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdResponsable = request.IdResponsable;
            entity.Titulo = request.Titulo;
            entity.Descripcion = request.Descripcion;
            entity.Estado = request.Estado;
            entity.CausaRaiz = request.CausaRaiz;
            entity.SolucionTemporal = request.SolucionTemporal;
            entity.SolucionDefinitiva = request.SolucionDefinitiva;

            await _unitOfWork.Complete();
            return new(200, new ProblemasResponseDto
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
            }, "Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarProblemasCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarProblemasCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
