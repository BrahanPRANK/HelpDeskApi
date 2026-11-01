using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Problemas.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Problemas;

public record ListarProblemasQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<ProblemasResponseDto>>>;

internal sealed class ListarProblemasQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarProblemasQueryHandler> _logger) : IRequestHandler<ListarProblemasQuery, ResponseDto<PaginaDto<ProblemasResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ProblemasResponseDto>>> Handle(ListarProblemasQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Problemas>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Titulo.Contains(options.Buscar) || x.Descripcion.Contains(options.Buscar));
            if (!string.IsNullOrWhiteSpace(options.Estado)) query = query.Where(x => x.Estado == options.Estado);
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdProblema).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new ProblemasResponseDto { IdProblema = entity.IdProblema, IdResponsable = entity.IdResponsable, Titulo = entity.Titulo, Descripcion = entity.Descripcion, Estado = entity.Estado, CausaRaiz = entity.CausaRaiz, SolucionTemporal = entity.SolucionTemporal, SolucionDefinitiva = entity.SolucionDefinitiva, FechaCreacion = entity.FechaCreacion }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarProblemasQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarProblemasQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
