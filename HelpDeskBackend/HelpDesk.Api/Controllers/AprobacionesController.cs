using HelpDesk.Application.UseCases.Aprobaciones;
using HelpDesk.Domain.Dtos.Aprobaciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Aprobaciones"), Authorize]
public sealed class AprobacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarAprobacionesQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idAprobacion:guid}")]
    public async Task<IActionResult> Get(Guid idAprobacion)
    { var result = await mediator.Send(new ObtenerAprobacionesQuery { IdAprobacion = idAprobacion }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAprobacionesCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idAprobacion:guid}")]
    public async Task<IActionResult> Actualizar(Guid idAprobacion, [FromBody] ActualizarAprobacionesCommand datos)
    {
        datos.Keys = [idAprobacion];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
