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
    public async Task<IActionResult> List([FromQuery] ListarTicketsArticulosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Get(Guid idTicket, Guid idArticulo)
    { var result = await mediator.Send(new ObtenerTicketsArticulosQuery { IdTicket = idTicket, IdArticulo = idArticulo }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTicketsArticulosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTicket, Guid idArticulo, [FromBody] ActualizarTicketsArticulosCommand datos)
    {
        datos.Keys = [idTicket, idArticulo];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idTicket:guid}/{idArticulo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTicket, Guid idArticulo)
    { var result = await mediator.Send(new EliminarTicketsArticulosCommand { Keys = [idTicket, idArticulo] }); return StatusCode(result.StatusCode, result); }

}
