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
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarRolesPermisosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idRol:guid}/{idPermiso:guid}")]
    public async Task<IActionResult> Get(Guid idRol, Guid idPermiso)
    { var result = await mediator.Send(new ObtenerRolesPermisosQuery([idRol, idPermiso])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] RolesPermisosInputDto datos)
    { var result = await mediator.Send(new CrearRolesPermisosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idRol:guid}/{idPermiso:guid}")]
    public async Task<IActionResult> Eliminar(Guid idRol, Guid idPermiso)
    { var result = await mediator.Send(new EliminarRolesPermisosCommand([idRol, idPermiso])); return StatusCode(result.StatusCode, result); }

}
