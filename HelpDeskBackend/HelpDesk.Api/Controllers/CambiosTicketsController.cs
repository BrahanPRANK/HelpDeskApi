using HelpDesk.Application.UseCases.CambiosTickets;
using HelpDesk.Application.UseCases.CambiosTickets.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/CambiosTickets"), Authorize]
public sealed class CambiosTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarCambiosTicketsQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idCambio:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Get(Guid idCambio, Guid idTicket, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerCambiosTicketsQuery([idCambio, idTicket]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CambiosTicketsInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearCambiosTicketsCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idCambio:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Eliminar(Guid idCambio, Guid idTicket, CancellationToken ct)
    { var result = await mediator.Send(new EliminarCambiosTicketsCommand([idCambio, idTicket]), ct); return StatusCode(result.StatusCode, result); }

}
