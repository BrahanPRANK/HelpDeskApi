using HelpDesk.Application.UseCases.Servicios;
using HelpDesk.Domain.Dtos.Servicios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Servicios"), Authorize]
public sealed class ServiciosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarServiciosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idServicio:guid}")]
    public async Task<IActionResult> Get(Guid idServicio)
    { var result = await mediator.Send(new ObtenerServiciosQuery { IdServicio = idServicio }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearServiciosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idServicio:guid}")]
    public async Task<IActionResult> Actualizar(Guid idServicio, [FromBody] ActualizarServiciosCommand datos)
    {
        datos.Keys = [idServicio];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idServicio:guid}")]
    public async Task<IActionResult> Eliminar(Guid idServicio)
    { var result = await mediator.Send(new EliminarServiciosCommand { Keys = [idServicio] }); return StatusCode(result.StatusCode, result); }

}
