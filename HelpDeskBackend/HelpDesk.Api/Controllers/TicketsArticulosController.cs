using HelpDesk.Application.UseCases.TicketsArticulos;
using HelpDesk.Domain.Dtos.TicketsArticulos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/TicketsArticulos"), Authorize]
public sealed class TicketsArticulosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarTicketsArticulosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Get(Guid idTicket, Guid idArticulo)
    { var result = await mediator.Send(new ObtenerTicketsArticulosQuery([idTicket, idArticulo])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] TicketsArticulosInputDto datos)
    { var result = await mediator.Send(new CrearTicketsArticulosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTicket, Guid idArticulo, [FromBody] TicketsArticulosInputDto datos)
    { var result = await mediator.Send(new ActualizarTicketsArticulosCommand([idTicket, idArticulo], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTicket, Guid idArticulo)
    { var result = await mediator.Send(new EliminarTicketsArticulosCommand([idTicket, idArticulo])); return StatusCode(result.StatusCode, result); }

}
