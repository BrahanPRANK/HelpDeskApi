using HelpDesk.Application.UseCases.ProblemasTickets;
using HelpDesk.Domain.Dtos.ProblemasTickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/ProblemasTickets"), Authorize]
public sealed class ProblemasTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarProblemasTicketsQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idProblema:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Get(Guid idProblema, Guid idTicket)
    { var result = await mediator.Send(new ObtenerProblemasTicketsQuery { IdProblema = idProblema, IdTicket = idTicket }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearProblemasTicketsCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idProblema:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Eliminar(Guid idProblema, Guid idTicket)
    { var result = await mediator.Send(new EliminarProblemasTicketsCommand { Keys = [idProblema, idTicket] }); return StatusCode(result.StatusCode, result); }

}
