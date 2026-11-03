using HelpDesk.Application.UseCases.Areas;
using HelpDesk.Domain.Dtos.Areas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Areas"), Authorize]
public sealed class AreasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarAreasQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idArea:guid}")]
    public async Task<IActionResult> Get(Guid idArea)
    { var result = await mediator.Send(new ObtenerAreasQuery { IdArea = idArea }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAreasCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idArea:guid}")]
    public async Task<IActionResult> Actualizar(Guid idArea, [FromBody] ActualizarAreasCommand datos)
    {
        datos.Keys = [idArea];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idArea:guid}")]
    public async Task<IActionResult> Eliminar(Guid idArea)
    { var result = await mediator.Send(new EliminarAreasCommand { Keys = [idArea] }); return StatusCode(result.StatusCode, result); }

}
