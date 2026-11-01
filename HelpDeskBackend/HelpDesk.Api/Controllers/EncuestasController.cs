using HelpDesk.Application.UseCases.Encuestas;
using HelpDesk.Application.UseCases.Encuestas.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Encuestas"), Authorize]
public sealed class EncuestasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarEncuestasQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idEncuesta:guid}")]
    public async Task<IActionResult> Get(Guid idEncuesta, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerEncuestasQuery([idEncuesta]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] EncuestasInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearEncuestasCommand(datos), ct); return StatusCode(result.StatusCode, result); }

}
