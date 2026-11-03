using HelpDesk.Application.UseCases.Tickets;
using HelpDesk.Application.Dtos.Tickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Tickets"), Authorize]
public sealed class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarTicketsQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idTicket:guid}")]
    public async Task<IActionResult> Get(Guid idTicket)
    { var result = await mediator.Send(new ObtenerTicketsQuery([idTicket])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] TicketsInputDto datos)
    { var result = await mediator.Send(new CrearTicketsCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idTicket:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTicket, [FromBody] TicketsInputDto datos)
    { var result = await mediator.Send(new ActualizarTicketsCommand([idTicket], datos)); return StatusCode(result.StatusCode, result); }

}
