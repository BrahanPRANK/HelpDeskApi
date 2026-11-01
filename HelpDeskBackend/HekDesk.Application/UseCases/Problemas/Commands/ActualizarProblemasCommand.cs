using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Problemas.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Problemas;

public record ActualizarProblemasCommand(object[] Keys, ProblemasInputDto Datos) : IRequest<ResponseDto<ProblemasResponseDto>>;

internal sealed class ActualizarProblemasCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ActualizarProblemasCommandHandler> _logger) : IRequestHandler<ActualizarProblemasCommand, ResponseDto<ProblemasResponseDto>>
{
    public async Task<ResponseDto<ProblemasResponseDto>> Handle(ActualizarProblemasCommand request, CancellationToken ct)
    {
        try
        {
            var input=request.Datos;
            Validation.Check(input);
            Validation.Require(access.Admin,"Esta operacion requiere administracion.",403);
            var entity=await _unitOfWork.Repository<global::HelpDesk.Domain.Models.Problemas>().FindAsync(request.Keys, ct)
            ?? throw new BusinessException(404,"Registro no encontrado.");
            access.EnsureRead(entity);

            entity.IdResponsable = input.IdResponsable;
            entity.Titulo = input.Titulo;
            entity.Descripcion = input.Descripcion;
            entity.Estado = input.Estado;
            entity.CausaRaiz = input.CausaRaiz;
            entity.SolucionTemporal = input.SolucionTemporal;
            entity.SolucionDefinitiva = input.SolucionDefinitiva;

            await _unitOfWork.Complete(ct);
            return new(200,new ProblemasResponseDto { IdProblema = entity.IdProblema, IdResponsable = entity.IdResponsable, Titulo = entity.Titulo, Descripcion = entity.Descripcion, Estado = entity.Estado, CausaRaiz = entity.CausaRaiz, SolucionTemporal = entity.SolucionTemporal, SolucionDefinitiva = entity.SolucionDefinitiva, FechaCreacion = entity.FechaCreacion },"Registro actualizado.");
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ActualizarProblemasCommandHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ActualizarProblemasCommandHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
