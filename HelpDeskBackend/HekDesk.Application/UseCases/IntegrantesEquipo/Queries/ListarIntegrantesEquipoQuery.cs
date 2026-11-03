using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.IntegrantesEquipo;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.IntegrantesEquipo;

public record ListarIntegrantesEquipoQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<IntegrantesEquipoResponseDto>>>;

internal sealed class ListarIntegrantesEquipoQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarIntegrantesEquipoQuery, ResponseDto<PaginaDto<IntegrantesEquipoResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<IntegrantesEquipoResponseDto>>> Handle(ListarIntegrantesEquipoQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.IntegrantesEquipo>().Query());

            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEquipo).ThenBy(x => x.IdUsuario).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new IntegrantesEquipoResponseDto
            {
                IdEquipo = entity.IdEquipo,
                IdUsuario = entity.IdUsuario,
                EsCoordinador = entity.EsCoordinador,
                Disponible = entity.Disponible,
                Activo = entity.Activo
            })).ToListAsync();
            return new ResponseDto<PaginaDto<IntegrantesEquipoResponseDto>>(200, new PaginaDto<IntegrantesEquipoResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<IntegrantesEquipoResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<IntegrantesEquipoResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
