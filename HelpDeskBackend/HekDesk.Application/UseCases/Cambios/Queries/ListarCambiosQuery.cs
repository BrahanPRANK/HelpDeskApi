using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Cambios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Cambios;

public class ListarCambiosQuery : IRequest<ResponseDto<PaginaDto<CambiosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public string? Estado { get; set; }
}

internal sealed class ListarCambiosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarCambiosQuery, ResponseDto<PaginaDto<CambiosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<CambiosResponseDto>>> Handle(ListarCambiosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Cambios>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Titulo.Contains(request.Buscar) || x.Descripcion.Contains(request.Buscar) || x.Riesgo.Contains(request.Buscar));
            if (!string.IsNullOrWhiteSpace(request.Estado)) query = query.Where(x => x.Estado == request.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdCambio).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new CambiosResponseDto
            {
                IdCambio = entity.IdCambio,
                IdSolicitante = entity.IdSolicitante,
                IdResponsable = entity.IdResponsable,
                Titulo = entity.Titulo,
                Descripcion = entity.Descripcion,
                Riesgo = entity.Riesgo,
                PlanEjecucion = entity.PlanEjecucion,
                PlanReversion = entity.PlanReversion,
                RequiereAprobacion = entity.RequiereAprobacion,
                Estado = entity.Estado,
                FechaProgramada = entity.FechaProgramada,
                FechaFinalizacion = entity.FechaFinalizacion,
                Resultado = entity.Resultado,
                FechaCreacion = entity.FechaCreacion
            })).ToListAsync();
            return new ResponseDto<PaginaDto<CambiosResponseDto>>(200, new PaginaDto<CambiosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<CambiosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<CambiosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
