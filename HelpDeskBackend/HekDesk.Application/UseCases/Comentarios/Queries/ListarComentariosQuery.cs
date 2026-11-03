using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Comentarios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Comentarios;

public class ListarComentariosQuery : IRequest<ResponseDto<PaginaDto<ComentariosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarComentariosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarComentariosQuery, ResponseDto<PaginaDto<ComentariosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ComentariosResponseDto>>> Handle(ListarComentariosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Comentarios>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.Mensaje.Contains(request.Buscar));
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdComentario).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new ComentariosResponseDto
            {
                IdComentario = entity.IdComentario,
                IdTicket = entity.IdTicket,
                IdAutor = entity.IdAutor,
                Mensaje = entity.Mensaje,
                EsInterno = entity.EsInterno,
                Fecha = entity.Fecha
            })).ToListAsync();
            return new ResponseDto<PaginaDto<ComentariosResponseDto>>(200, new PaginaDto<ComentariosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<ComentariosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<ComentariosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
