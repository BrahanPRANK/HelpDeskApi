using HelpDesk.Application.UseCases.PoliticasAtencion;
using HelpDesk.Domain.Dtos.PoliticasAtencion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/PoliticasAtencion"), Authorize]
public sealed class PoliticasAtencionController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarPoliticasAtencionQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idPolitica:guid}")]
    public async Task<IActionResult> Get(Guid idPolitica)
    { var result = await mediator.Send(new ObtenerPoliticasAtencionQuery { IdPolitica = idPolitica }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPoliticasAtencionCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idPolitica:guid}")]
    public async Task<IActionResult> Actualizar(Guid idPolitica, [FromBody] ActualizarPoliticasAtencionCommand datos)
    {
        datos.Keys = [idPolitica];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idPolitica:guid}")]
    public async Task<IActionResult> Eliminar(Guid idPolitica)
    { var result = await mediator.Send(new EliminarPoliticasAtencionCommand { Keys = [idPolitica] }); return StatusCode(result.StatusCode, result); }

}
