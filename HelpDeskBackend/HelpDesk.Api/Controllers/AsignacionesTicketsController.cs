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
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarAsignacionesTicketsQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAsignacionTicket:guid}")]
    public async Task<IActionResult> Get(Guid idAsignacionTicket)
    { var result = await mediator.Send(new ObtenerAsignacionesTicketsQuery([idAsignacionTicket])); return StatusCode(result.StatusCode, result); }

}
