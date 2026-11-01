using HelpDesk.Application.UseCases.HistorialTickets;
using HelpDesk.Application.UseCases.HistorialTickets.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/HistorialTickets"), Authorize]
public sealed class HistorialTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarHistorialTicketsQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idHistorial:guid}")]
    public async Task<IActionResult> Get(Guid idHistorial, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerHistorialTicketsQuery([idHistorial]), ct); return StatusCode(result.StatusCode, result); }

}
