using HelpDesk.Application.UseCases.EquiposSoporte;
using HelpDesk.Application.UseCases.EquiposSoporte.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/EquiposSoporte"), Authorize]
public sealed class EquiposSoporteController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarEquiposSoporteQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idEquipo:guid}")]
    public async Task<IActionResult> Get(Guid idEquipo, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerEquiposSoporteQuery([idEquipo]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] EquiposSoporteInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearEquiposSoporteCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idEquipo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idEquipo, [FromBody] EquiposSoporteInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarEquiposSoporteCommand([idEquipo], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idEquipo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idEquipo, CancellationToken ct)
    { var result = await mediator.Send(new EliminarEquiposSoporteCommand([idEquipo]), ct); return StatusCode(result.StatusCode, result); }

}
