using HelpDesk.Application.UseCases.EnviosNotificaciones;
using HelpDesk.Application.Dtos.EnviosNotificaciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/EnviosNotificaciones"), Authorize]
public sealed class EnviosNotificacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarEnviosNotificacionesQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idEnvio:guid}")]
    public async Task<IActionResult> Get(Guid idEnvio)
    { var result = await mediator.Send(new ObtenerEnviosNotificacionesQuery([idEnvio])); return StatusCode(result.StatusCode, result); }

}
