using HelpDesk.Application.UseCases.Permisos;
using HelpDesk.Application.UseCases.Permisos.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Permisos"), Authorize]
public sealed class PermisosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarPermisosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idPermiso:guid}")]
    public async Task<IActionResult> Get(Guid idPermiso, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerPermisosQuery([idPermiso]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] PermisosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearPermisosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idPermiso:guid}")]
    public async Task<IActionResult> Actualizar(Guid idPermiso, [FromBody] PermisosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarPermisosCommand([idPermiso], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idPermiso:guid}")]
    public async Task<IActionResult> Eliminar(Guid idPermiso, CancellationToken ct)
    { var result = await mediator.Send(new EliminarPermisosCommand([idPermiso]), ct); return StatusCode(result.StatusCode, result); }

}
