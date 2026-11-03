using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Areas;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Areas;

public record ListarAreasQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<AreasResponseDto>>>;

internal sealed class ListarAreasQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarAreasQuery, ResponseDto<PaginaDto<AreasResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<AreasResponseDto>>> Handle(ListarAreasQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Areas>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombre.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdArea).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new AreasResponseDto
            {
                IdArea = entity.IdArea,
                IdSede = entity.IdSede,
                Nombre = entity.Nombre,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<AreasResponseDto>>(200, new PaginaDto<AreasResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<AreasResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<AreasResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
