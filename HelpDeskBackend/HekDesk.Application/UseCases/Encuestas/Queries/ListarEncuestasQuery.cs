using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Encuestas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Encuestas;

public class ListarEncuestasQuery : IRequest<ResponseDto<PaginaDto<EncuestasResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public Guid? IdTicket { get; set; }
}

internal sealed class ListarEncuestasQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarEncuestasQuery, ResponseDto<PaginaDto<EncuestasResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<EncuestasResponseDto>>> Handle(ListarEncuestasQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Encuestas>().Query());
            if (request.IdTicket.HasValue) query = query.Where(x => x.IdTicket == request.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEncuesta).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new EncuestasResponseDto
            {
                IdEncuesta = entity.IdEncuesta,
                IdTicket = entity.IdTicket,
                IdUsuario = entity.IdUsuario,
                Calificacion = entity.Calificacion,
                Comentario = entity.Comentario,
                FechaRespuesta = entity.FechaRespuesta
            })).ToListAsync();
            return new ResponseDto<PaginaDto<EncuestasResponseDto>>(200, new PaginaDto<EncuestasResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<EncuestasResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<EncuestasResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
