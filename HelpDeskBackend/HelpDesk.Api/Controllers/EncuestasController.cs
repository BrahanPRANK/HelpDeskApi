using HelpDesk.Application.UseCases.Encuestas;
using HelpDesk.Application.Dtos.Encuestas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Encuestas"), Authorize]
public sealed class EncuestasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarEncuestasQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idEncuesta:guid}")]
    public async Task<IActionResult> Get(Guid idEncuesta)
    { var result = await mediator.Send(new ObtenerEncuestasQuery([idEncuesta])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] EncuestasInputDto datos)
    { var result = await mediator.Send(new CrearEncuestasCommand(datos)); return StatusCode(result.StatusCode, result); }

}
