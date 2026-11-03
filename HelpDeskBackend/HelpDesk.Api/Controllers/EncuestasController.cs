using HelpDesk.Application.UseCases.Encuestas;
using HelpDesk.Domain.Dtos.Encuestas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Encuestas"), Authorize]
public sealed class EncuestasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarEncuestasQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idEncuesta:guid}")]
    public async Task<IActionResult> Get(Guid idEncuesta)
    { var result = await mediator.Send(new ObtenerEncuestasQuery { IdEncuesta = idEncuesta }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEncuestasCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
