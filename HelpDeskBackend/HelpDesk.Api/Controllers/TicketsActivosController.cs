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
    public async Task<IActionResult> List([FromQuery] ListarTicketsActivosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idTicket:guid}/{idActivo:guid}")]
    public async Task<IActionResult> Get(Guid idTicket, Guid idActivo)
    { var result = await mediator.Send(new ObtenerTicketsActivosQuery { IdTicket = idTicket, IdActivo = idActivo }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTicketsActivosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idTicket:guid}/{idActivo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTicket, Guid idActivo)
    { var result = await mediator.Send(new EliminarTicketsActivosCommand { Keys = [idTicket, idActivo] }); return StatusCode(result.StatusCode, result); }

}
