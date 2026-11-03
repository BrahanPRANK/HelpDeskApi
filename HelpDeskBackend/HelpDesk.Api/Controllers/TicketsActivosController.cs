using HelpDesk.Application.UseCases.TicketsActivos;
using HelpDesk.Domain.Dtos.TicketsActivos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/TicketsActivos"), Authorize]
public sealed class TicketsActivosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarTicketsActivosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idTicket:guid}/{idActivo:guid}")]
    public async Task<IActionResult> Get(Guid idTicket, Guid idActivo)
    { var result = await mediator.Send(new ObtenerTicketsActivosQuery([idTicket, idActivo])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] TicketsActivosInputDto datos)
    { var result = await mediator.Send(new CrearTicketsActivosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idTicket:guid}/{idActivo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTicket, Guid idActivo)
    { var result = await mediator.Send(new EliminarTicketsActivosCommand([idTicket, idActivo])); return StatusCode(result.StatusCode, result); }

}
