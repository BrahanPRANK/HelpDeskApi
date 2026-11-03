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

public class ListarPoliticasAtencionQuery : IRequest<ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
}

internal sealed class ListarPoliticasAtencionQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarPoliticasAtencionQuery, ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>> Handle(ListarPoliticasAtencionQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.PoliticasAtencion>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdPolitica).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new PoliticasAtencionResponseDto
            {
                IdPolitica = entity.IdPolitica,
                Prioridad = entity.Prioridad,
                MinutosPrimeraRespuesta = entity.MinutosPrimeraRespuesta,
                MinutosResolucion = entity.MinutosResolucion,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<PoliticasAtencionResponseDto>>(200, new PaginaDto<PoliticasAtencionResponseDto>(items, request.Pagina, request.Tamano, total));
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
