using HelpDesk.Application.UseCases.TicketsArticulos;
using HelpDesk.Application.UseCases.TicketsArticulos.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/TicketsArticulos"), Authorize]
public sealed class TicketsArticulosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarTicketsArticulosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Get(Guid idTicket, Guid idArticulo, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerTicketsArticulosQuery([idTicket, idArticulo]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] TicketsArticulosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearTicketsArticulosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTicket, Guid idArticulo, [FromBody] TicketsArticulosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarTicketsArticulosCommand([idTicket, idArticulo], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTicket, Guid idArticulo, CancellationToken ct)
    { var result = await mediator.Send(new EliminarTicketsArticulosCommand([idTicket, idArticulo]), ct); return StatusCode(result.StatusCode, result); }

}
