using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Auditoria;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Auditoria;

public class ListarAuditoriaQuery : IRequest<ResponseDto<PaginaDto<AuditoriaResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
}

internal sealed class ListarAuditoriaQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAuditoriaQuery, ResponseDto<PaginaDto<AuditoriaResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AuditoriaResponseDto>>> Handle(ListarAuditoriaQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Auditoria>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Accion.Contains(request.Buscar) || x.TablaAfectada.Contains(request.Buscar) || x.Descripcion.Contains(request.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAuditoria).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new AuditoriaResponseDto
            {
                IdAuditoria = entity.IdAuditoria,
                IdUsuario = entity.IdUsuario,
                Accion = entity.Accion,
                TablaAfectada = entity.TablaAfectada,
                IdRegistro = entity.IdRegistro,
                Descripcion = entity.Descripcion,
                Fecha = entity.Fecha
            })).ToListAsync();
            return new ResponseDto<PaginaDto<AuditoriaResponseDto>>(200, new PaginaDto<AuditoriaResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<AuditoriaResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<AuditoriaResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
