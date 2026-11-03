using HelpDesk.Application.UseCases.HistorialTickets;
using HelpDesk.Domain.Dtos.HistorialTickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/HistorialTickets"), Authorize]
public sealed class HistorialTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarHistorialTicketsQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idHistorial:guid}")]
    public async Task<IActionResult> Get(Guid idHistorial)
    { var result = await mediator.Send(new ObtenerHistorialTicketsQuery { IdHistorial = idHistorial }); return StatusCode(result.StatusCode, result); }

}
