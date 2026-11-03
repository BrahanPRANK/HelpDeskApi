using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.PoliticasAtencion;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.PoliticasAtencion;

public record ListarPoliticasAtencionQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>>;

internal sealed class ListarPoliticasAtencionQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarPoliticasAtencionQuery, ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>> Handle(ListarPoliticasAtencionQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdPolitica).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new PoliticasAtencionResponseDto
            {
                IdPolitica = entity.IdPolitica,
                Prioridad = entity.Prioridad,
                MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta,
                MinutosResolucion = entity.MinutosResolucion,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>(200, new PaginaDto<PoliticasAtencionResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
