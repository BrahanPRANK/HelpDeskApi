using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PreferenciasNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PreferenciasNotificaciones;

public class ListarPreferenciasNotificacionesQuery : IRequest<ResponseDto<PaginaDto<PreferenciasNotificacionesResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
}

internal sealed class ListarPreferenciasNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarPreferenciasNotificacionesQuery, ResponseDto<PaginaDto<PreferenciasNotificacionesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<PreferenciasNotificacionesResponseDto>>> Handle(ListarPreferenciasNotificacionesQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.PreferenciasNotificaciones>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.TipoEvento.Contains(request.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdUsuario).ThenBy(x => x.TipoEvento).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new PreferenciasNotificacionesResponseDto
            {
                IdUsuario = entity.IdUsuario,
                TipoEvento = entity.TipoEvento,
                RecibirEnSistema = entity.RecibirEnSistema,
                RecibirPorCorreo = entity.RecibirPorCorreo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<PreferenciasNotificacionesResponseDto>>(200, new PaginaDto<PreferenciasNotificacionesResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<PreferenciasNotificacionesResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<PreferenciasNotificacionesResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
