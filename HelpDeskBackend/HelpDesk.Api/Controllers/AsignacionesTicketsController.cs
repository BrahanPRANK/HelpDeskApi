using HelpDesk.Application.UseCases.AsignacionesTickets;
using HelpDesk.Domain.Dtos.AsignacionesTickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/AsignacionesTickets"), Authorize]
public sealed class AsignacionesTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarAsignacionesTicketsQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idAsignacionTicket:guid}")]
    public async Task<IActionResult> Get(Guid idAsignacionTicket)
    { var result = await mediator.Send(new ObtenerAsignacionesTicketsQuery { IdAsignacionTicket = idAsignacionTicket }); return StatusCode(result.StatusCode, result); }

}
