using HelpDesk.Application.UseCases.ArticulosConocimiento;
using HelpDesk.Domain.Dtos.ArticulosConocimiento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/ArticulosConocimiento"), Authorize]
public sealed class ArticulosConocimientoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarArticulosConocimientoQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idArticulo:guid}")]
    public async Task<IActionResult> Get(Guid idArticulo)
    { var result = await mediator.Send(new ObtenerArticulosConocimientoQuery { IdArticulo = idArticulo }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearArticulosConocimientoCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idArticulo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idArticulo, [FromBody] ActualizarArticulosConocimientoCommand datos)
    {
        datos.Keys = [idArticulo];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idArticulo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idArticulo)
    { var result = await mediator.Send(new EliminarArticulosConocimientoCommand { Keys = [idArticulo] }); return StatusCode(result.StatusCode, result); }

}
