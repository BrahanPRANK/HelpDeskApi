using HelpDesk.Application.UseCases.UsuariosRoles;
using HelpDesk.Domain.Dtos.UsuariosRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/UsuariosRoles"), Authorize]
public sealed class UsuariosRolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarUsuariosRolesQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idUsuario:guid}/{idRol:guid}")]
    public async Task<IActionResult> Get(Guid idUsuario, Guid idRol)
    { var result = await mediator.Send(new ObtenerUsuariosRolesQuery { IdUsuario = idUsuario, IdRol = idRol }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuariosRolesCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idUsuario:guid}/{idRol:guid}")]
    public async Task<IActionResult> Actualizar(Guid idUsuario, Guid idRol, [FromBody] ActualizarUsuariosRolesCommand datos)
    {
        datos.Keys = [idUsuario, idRol];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idUsuario:guid}/{idRol:guid}")]
    public async Task<IActionResult> Eliminar(Guid idUsuario, Guid idRol)
    { var result = await mediator.Send(new EliminarUsuariosRolesCommand { Keys = [idUsuario, idRol] }); return StatusCode(result.StatusCode, result); }

}
