using HelpDesk.Application.UseCases.Adjuntos;
using HelpDesk.Domain.Dtos.Adjuntos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Adjuntos"), Authorize]
public sealed class AdjuntosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarAdjuntosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idAdjunto:guid}")]
    public async Task<IActionResult> Get(Guid idAdjunto)
    { var result = await mediator.Send(new ObtenerAdjuntosQuery { IdAdjunto = idAdjunto }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAdjuntosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idAdjunto:guid}")]
    public async Task<IActionResult> Actualizar(Guid idAdjunto, [FromBody] ActualizarAdjuntosCommand datos)
    {
        datos.Keys = [idAdjunto];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idAdjunto:guid}")]
    public async Task<IActionResult> Eliminar(Guid idAdjunto)
    { var result = await mediator.Send(new EliminarAdjuntosCommand { Keys = [idAdjunto] }); return StatusCode(result.StatusCode, result); }

}
