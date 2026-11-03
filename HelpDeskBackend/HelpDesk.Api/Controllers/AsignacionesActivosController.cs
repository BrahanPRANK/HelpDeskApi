using HelpDesk.Application.UseCases.AsignacionesActivos;
using HelpDesk.Domain.Dtos.AsignacionesActivos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/AsignacionesActivos"), Authorize]
public sealed class AsignacionesActivosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarAsignacionesActivosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idAsignacion:guid}")]
    public async Task<IActionResult> Get(Guid idAsignacion)
    { var result = await mediator.Send(new ObtenerAsignacionesActivosQuery { IdAsignacion = idAsignacion }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAsignacionesActivosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
