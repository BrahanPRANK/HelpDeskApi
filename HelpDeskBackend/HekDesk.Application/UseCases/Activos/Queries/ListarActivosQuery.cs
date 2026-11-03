using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Activos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Activos;

public class ListarActivosQuery : IRequest<ResponseDto<PaginaDto<ActivosResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Buscar { get; set; }
    public string? Estado { get; set; }
}

internal sealed class ListarActivosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarActivosQuery, ResponseDto<PaginaDto<ActivosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<ActivosResponseDto>>> Handle(ListarActivosQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Activos>().Query());
            if (!string.IsNullOrWhiteSpace(request.Buscar)) query = query.Where(x => x.CodigoPatrimonial.Contains(request.Buscar) || x.Nombre.Contains(request.Buscar));
            if (!string.IsNullOrWhiteSpace(request.Estado)) query = query.Where(x => x.Estado == request.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdActivo).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new ActivosResponseDto
            {
                IdActivo = entity.IdActivo,
                IdSede = entity.IdSede,
                IdArea = entity.IdArea,
                CodigoPatrimonial = entity.CodigoPatrimonial,
                Nombre = entity.Nombre,
                IdTipoActivo = entity.IdTipoActivo,
                NumeroSerie = entity.NumeroSerie,
                Marca = entity.Marca,
                Modelo = entity.Modelo,
                Estado = entity.Estado,
                FechaRegistro = entity.FechaRegistro
            })).ToListAsync();
            return new ResponseDto<PaginaDto<ActivosResponseDto>>(200, new PaginaDto<ActivosResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<ActivosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<ActivosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
