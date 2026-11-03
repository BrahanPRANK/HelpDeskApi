using HelpDesk.Application.UseCases.Tickets;
using HelpDesk.Domain.Dtos.Tickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Tickets"), Authorize]
public sealed class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarTicketsQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idTicket:guid}")]
    public async Task<IActionResult> Get(Guid idTicket)
    { var result = await mediator.Send(new ObtenerTicketsQuery { IdTicket = idTicket }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTicketsCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idTicket:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTicket, [FromBody] ActualizarTicketsCommand datos)
    {
        datos.Keys = [idTicket];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
