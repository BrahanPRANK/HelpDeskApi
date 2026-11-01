using HelpDesk.Application.UseCases.Roles;
using HelpDesk.Application.UseCases.Roles.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Roles"), Authorize]
public sealed class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarRolesQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idRol:guid}")]
    public async Task<IActionResult> Get(Guid idRol, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerRolesQuery([idRol]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] RolesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearRolesCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idRol:guid}")]
    public async Task<IActionResult> Actualizar(Guid idRol, [FromBody] RolesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarRolesCommand([idRol], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idRol:guid}")]
    public async Task<IActionResult> Eliminar(Guid idRol, CancellationToken ct)
    { var result = await mediator.Send(new EliminarRolesCommand([idRol]), ct); return StatusCode(result.StatusCode, result); }

}
