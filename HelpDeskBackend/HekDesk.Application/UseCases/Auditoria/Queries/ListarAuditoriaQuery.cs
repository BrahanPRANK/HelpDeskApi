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

public record ListarAuditoriaQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<AuditoriaResponseDto>>>;

internal sealed class ListarAuditoriaQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAuditoriaQuery, ResponseDto<PaginaDto<AuditoriaResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AuditoriaResponseDto>>> Handle(ListarAuditoriaQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Auditoria>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Accion.Contains(options.Buscar) || x.TablaAfectada.Contains(options.Buscar) || x.Descripcion.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdAuditoria).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
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
            return new ResponseDto<PaginaDto<AuditoriaResponseDto>>(200, new PaginaDto<AuditoriaResponseDto>(items, options.Pagina, options.Tamano, total));
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
