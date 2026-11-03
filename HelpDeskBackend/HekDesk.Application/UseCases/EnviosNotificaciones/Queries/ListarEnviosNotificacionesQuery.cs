using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.EnviosNotificaciones;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.EnviosNotificaciones;

public class ListarEnviosNotificacionesQuery : IRequest<ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>>
{
    public int Pagina { get; set; } = 1;
    public int Tamano { get; set; } = 20;
    public string? Estado { get; set; }
}

internal sealed class ListarEnviosNotificacionesQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarEnviosNotificacionesQuery, ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>> Handle(ListarEnviosNotificacionesQuery request, CancellationToken ct)
    {
        try
        {

            Validation.Require(request.Pagina >= 1 && request.Pagina <= 1000000 && request.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.EnviosNotificaciones>().Query());
            if (!string.IsNullOrWhiteSpace(request.Estado)) query = query.Where(x => x.Estado == request.Estado);
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdEnvio).Skip((request.Pagina - 1) * request.Tamano).Take(request.Tamano)
            .Select(entity => new EnviosNotificacionesResponseDto
            {
                IdEnvio = entity.IdEnvio,
                IdNotificacion = entity.IdNotificacion,
                CorreoDestino = entity.CorreoDestino,
                Estado = entity.Estado,
                Intentos = entity.Intentos,
                MaximoIntentos = entity.MaximoIntentos,
                FechaCreacion = entity.FechaCreacion,
                FechaProximoIntento = entity.FechaProximoIntento,
                BloqueoHasta = entity.BloqueoHasta,
                FechaEnvio = entity.FechaEnvio,
                IdentificadorProveedor = entity.IdentificadorProveedor
            })).ToListAsync();
            return new ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>(200, new PaginaDto<EnviosNotificacionesResponseDto>(items, request.Pagina, request.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<EnviosNotificacionesResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
