using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Notificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Notificaciones;

public record ListarNotificacionesQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<NotificacionesResponseDto>>>;

internal sealed class ListarNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarNotificacionesQuery, ResponseDto<PaginaDto<NotificacionesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<NotificacionesResponseDto>>> Handle(ListarNotificacionesQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Notificaciones>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Titulo.Contains(options.Buscar) || x.Mensaje.Contains(options.Buscar) || x.ClaveEvento.Contains(options.Buscar));
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdNotificacion).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new NotificacionesResponseDto
            {
                IdNotificacion = entity.IdNotificacion,
                IdUsuario = entity.IdUsuario,
                IdTicket = entity.IdTicket,
                Titulo = entity.Titulo,
                Mensaje = entity.Mensaje,
                FechaCreacion = entity.FechaCreacion,
                FechaLectura = entity.FechaLectura,
                ClaveEvento = entity.ClaveEvento,
                TipoEvento = entity.TipoEvento
            })).ToListAsync();
            return new ResponseDto<PaginaDto<NotificacionesResponseDto>>(200, new PaginaDto<NotificacionesResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<NotificacionesResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<NotificacionesResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
