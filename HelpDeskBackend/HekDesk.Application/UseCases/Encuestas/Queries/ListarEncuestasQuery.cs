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

public record ListarEncuestasQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<EncuestasResponseDto>>>;

internal sealed class ListarEncuestasQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarEncuestasQuery, ResponseDto<PaginaDto<EncuestasResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<EncuestasResponseDto>>> Handle(ListarEncuestasQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Encuestas>().Query());
            if (options.IdTicket.HasValue) query = query.Where(x => x.IdTicket == options.IdTicket.Value);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEncuesta).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new EncuestasResponseDto
            {
                IdEncuesta = entity.IdEncuesta,
                IdTicket = entity.IdTicket,
                IdUsuario = entity.IdUsuario,
                Calificacion = entity.Calificacion,
                Comentario = entity.Comentario,
                FechaRespuesta = entity.FechaRespuesta
            })).ToListAsync();
            return new ResponseDto<PaginaDto<EncuestasResponseDto>>(200, new PaginaDto<EncuestasResponseDto>(items, options.Pagina, options.Tamano, total));
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
