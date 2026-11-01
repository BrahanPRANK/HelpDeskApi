using HelpDesk.Application.UseCases.Problemas;
using HelpDesk.Application.UseCases.Problemas.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Problemas"), Authorize]
public sealed class ProblemasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarProblemasQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idProblema:guid}")]
    public async Task<IActionResult> Get(Guid idProblema, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerProblemasQuery([idProblema]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ProblemasInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearProblemasCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idProblema:guid}")]
    public async Task<IActionResult> Actualizar(Guid idProblema, [FromBody] ProblemasInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarProblemasCommand([idProblema], datos), ct); return StatusCode(result.StatusCode, result); }

}
