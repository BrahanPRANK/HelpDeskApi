using HelpDesk.Application.UseCases.ProblemasTickets;
using HelpDesk.Application.Dtos.ProblemasTickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/ProblemasTickets"), Authorize]
public sealed class ProblemasTicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarProblemasTicketsQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idProblema:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Get(Guid idProblema, Guid idTicket)
    { var result = await mediator.Send(new ObtenerProblemasTicketsQuery([idProblema, idTicket])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ProblemasTicketsInputDto datos)
    { var result = await mediator.Send(new CrearProblemasTicketsCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idProblema:guid}/{idTicket:guid}")]
    public async Task<IActionResult> Eliminar(Guid idProblema, Guid idTicket)
    { var result = await mediator.Send(new EliminarProblemasTicketsCommand([idProblema, idTicket])); return StatusCode(result.StatusCode, result); }

}
