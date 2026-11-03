using HelpDesk.Application.UseCases.CambiosTickets;
using HelpDesk.Domain.Dtos.CambiosTickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/CambiosTickets"), Authorize]
public sealed class CambiosTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarCambiosTicketsQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idCambio:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Get(Guid idCambio, Guid idTicket)
    { var result = await mediator.Send(new ObtenerCambiosTicketsQuery([idCambio, idTicket])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCambiosTicketsCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idCambio:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Eliminar(Guid idCambio, Guid idTicket)
    { var result = await mediator.Send(new EliminarCambiosTicketsCommand { Keys = [idCambio, idTicket] }); return StatusCode(result.StatusCode, result); }

}
