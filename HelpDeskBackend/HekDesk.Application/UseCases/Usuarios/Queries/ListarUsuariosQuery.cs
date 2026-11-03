using HelpDesk.Domain.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.Usuarios;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.Usuarios;

public record ListarUsuariosQuery(ListOptions Options) : IRequest<ResponseDto<PaginaDto<UsuariosResponseDto>>>;

internal sealed class ListarUsuariosQueryHandler(IUnitOfWork _unitOfWork, AccessRules access) : IRequestHandler<ListarUsuariosQuery, ResponseDto<PaginaDto<UsuariosResponseDto>>>
{
    public async Task<ResponseDto<PaginaDto<UsuariosResponseDto>>> Handle(ListarUsuariosQuery request, CancellationToken ct)
    {
        try
        {
            var options = request.Options;
            Validation.Require(options.Pagina >= 1 && options.Pagina <= 1000000 && options.Tamano is >= 1 and <= 100,
            "Pagina debe ser positiva y tamano debe estar entre 1 y 100.");
            var query = access.Scope(_unitOfWork.Repository<global::HelpDesk.Domain.Models.Usuarios>().Query());
            if (!string.IsNullOrWhiteSpace(options.Buscar)) query = query.Where(x => x.Nombres.Contains(options.Buscar) || x.Apellidos.Contains(options.Buscar) || x.NombreUsuario.Contains(options.Buscar));
            var total = await (query).CountAsync();
            var items = await (query.OrderBy(x => x.IdUsuario).Skip((options.Pagina - 1) * options.Tamano).Take(options.Tamano)
            .Select(entity => new UsuariosResponseDto
            {
                IdUsuario = entity.IdUsuario,
                IdArea = entity.IdArea,
                Nombres = entity.Nombres,
                Apellidos = entity.Apellidos,
                NombreUsuario = entity.NombreUsuario,
                Correo = entity.Correo,
                Telefono = entity.Telefono,
                Activo = entity.Activo,
                FechaRegistro = entity.FechaRegistro
            })).ToListAsync();
            return new ResponseDto<PaginaDto<UsuariosResponseDto>>(200, new PaginaDto<UsuariosResponseDto>(items, options.Pagina, options.Tamano, total));
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<PaginaDto<UsuariosResponseDto>>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<PaginaDto<UsuariosResponseDto>>(500, default, "No se pudo completar la operacion.");
        }
    }

}
