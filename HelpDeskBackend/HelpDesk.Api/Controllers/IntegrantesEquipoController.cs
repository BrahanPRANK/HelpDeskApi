using HelpDesk.Application.UseCases.IntegrantesEquipo;
using HelpDesk.Domain.Dtos.IntegrantesEquipo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/IntegrantesEquipo"), Authorize]
public sealed class IntegrantesEquipoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarIntegrantesEquipoQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Get(Guid idEquipo, Guid idUsuario)
    { var result = await mediator.Send(new ObtenerIntegrantesEquipoQuery { IdEquipo = idEquipo, IdUsuario = idUsuario }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearIntegrantesEquipoCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idEquipo, Guid idUsuario, [FromBody] ActualizarIntegrantesEquipoCommand datos)
    {
        datos.Keys = [idEquipo, idUsuario];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Eliminar(Guid idEquipo, Guid idUsuario)
    { var result = await mediator.Send(new EliminarIntegrantesEquipoCommand { Keys = [idEquipo, idUsuario] }); return StatusCode(result.StatusCode, result); }

}
