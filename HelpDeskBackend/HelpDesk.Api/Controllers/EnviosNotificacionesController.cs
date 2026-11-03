using HelpDesk.Application.UseCases.EnviosNotificaciones;
using HelpDesk.Domain.Dtos.EnviosNotificaciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/EnviosNotificaciones"), Authorize]
public sealed class EnviosNotificacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarEnviosNotificacionesQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idEnvio:guid}")]
    public async Task<IActionResult> Get(Guid idEnvio)
    { var result = await mediator.Send(new ObtenerEnviosNotificacionesQuery { IdEnvio = idEnvio }); return StatusCode(result.StatusCode, result); }

}
