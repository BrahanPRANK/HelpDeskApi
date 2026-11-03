using HelpDesk.Application.UseCases.Activos;
using HelpDesk.Domain.Dtos.Activos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Activos"), Authorize]
public sealed class ActivosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarActivosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idActivo:guid}")]
    public async Task<IActionResult> Get(Guid idActivo)
    { var result = await mediator.Send(new ObtenerActivosQuery { IdActivo = idActivo }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearActivosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idActivo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idActivo, [FromBody] ActualizarActivosCommand datos)
    {
        datos.Keys = [idActivo];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idActivo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idActivo)
    { var result = await mediator.Send(new EliminarActivosCommand { Keys = [idActivo] }); return StatusCode(result.StatusCode, result); }

}
