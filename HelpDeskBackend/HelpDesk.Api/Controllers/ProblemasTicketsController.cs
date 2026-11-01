using HelpDesk.Application.UseCases.ProblemasTickets;
using HelpDesk.Application.UseCases.ProblemasTickets.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/ProblemasTickets"), Authorize]
public sealed class ProblemasTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarProblemasTicketsQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idProblema:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Get(Guid idProblema, Guid idTicket, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerProblemasTicketsQuery([idProblema, idTicket]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ProblemasTicketsInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearProblemasTicketsCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idProblema:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Eliminar(Guid idProblema, Guid idTicket, CancellationToken ct)
    { var result = await mediator.Send(new EliminarProblemasTicketsCommand([idProblema, idTicket]), ct); return StatusCode(result.StatusCode, result); }

}
