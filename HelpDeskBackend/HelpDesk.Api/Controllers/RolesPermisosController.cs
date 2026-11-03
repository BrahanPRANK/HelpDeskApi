using HelpDesk.Application.UseCases.RolesPermisos;
using HelpDesk.Domain.Dtos.RolesPermisos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/RolesPermisos"), Authorize]
public sealed class RolesPermisosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarRolesPermisosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idRol:guid}/{idPermiso:guid}")]
    public async Task<IActionResult> Get(Guid idRol, Guid idPermiso)
    { var result = await mediator.Send(new ObtenerRolesPermisosQuery { IdRol = idRol, IdPermiso = idPermiso }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearRolesPermisosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idRol:guid}/{idPermiso:guid}")]
    public async Task<IActionResult> Eliminar(Guid idRol, Guid idPermiso)
    { var result = await mediator.Send(new EliminarRolesPermisosCommand { Keys = [idRol, idPermiso] }); return StatusCode(result.StatusCode, result); }

}
