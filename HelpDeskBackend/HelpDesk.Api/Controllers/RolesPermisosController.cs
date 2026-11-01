using HelpDesk.Application.UseCases.RolesPermisos;
using HelpDesk.Application.UseCases.RolesPermisos.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/RolesPermisos"), Authorize]
public sealed class RolesPermisosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarRolesPermisosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idRol:guid}/{idPermiso:guid}")]
    public async Task<IActionResult> Get(Guid idRol, Guid idPermiso, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerRolesPermisosQuery([idRol, idPermiso]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] RolesPermisosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearRolesPermisosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idRol:guid}/{idPermiso:guid}")]
    public async Task<IActionResult> Eliminar(Guid idRol, Guid idPermiso, CancellationToken ct)
    { var result = await mediator.Send(new EliminarRolesPermisosCommand([idRol, idPermiso]), ct); return StatusCode(result.StatusCode, result); }

}
