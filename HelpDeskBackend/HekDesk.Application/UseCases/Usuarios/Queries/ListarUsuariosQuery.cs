using Microsoft.Extensions.Logging;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Application.UseCases.Usuarios.Dtos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios;

public record ListarUsuariosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<UsuariosResponseDto>>>;

internal sealed class ListarUsuariosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access, ILogger<ListarUsuariosQueryHandler> _logger) : IRequestHandler<ListarUsuariosQuery, ResponseDto<PaginaDto<UsuariosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<UsuariosResponseDto>>> Handle(ListarUsuariosQuery request, CancellationToken ct)
    {
        try
        {
            var options=request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query=access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombres.Contains(options.Buscar) || x.Apellidos.Contains(options.Buscar) || x.NombreUsuario.Contains(options.Buscar));
            var total=await _unitOfWork.CountAsync(query,ct);
            var items=await _unitOfWork.ListAsync(query.OrderBy(x => x.IdUsuario).Skip((options.Pagina-1)*options.Tamano).Take(options.Tamano)
            .Select(entity => new UsuariosResponseDto { IdUsuario = entity.IdUsuario, IdArea = entity.IdArea, Nombres = entity.Nombres, Apellidos = entity.Apellidos, NombreUsuario = entity.NombreUsuario, Correo = entity.Correo, Telefono = entity.Telefono, Activo = entity.Activo, FechaRegistro = entity.FechaRegistro }),ct);
            return new(200,new(items,options.Pagina,options.Tamano,total));
        }
        catch (BusinessException ex)
        {
            if (ex.Status >= 500)
                _logger.LogError(ex, "Error controlado en ListarUsuariosQueryHandler.");
            return new(ex.Status, default, ex.Message);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ejecutar ListarUsuariosQueryHandler.");
            return new(500, default, "No se pudo completar la operacion.");
        }
    }

}
