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
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarServiciosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idServicio:guid}")]
    public async Task<IActionResult> Get(Guid idServicio)
    { var result = await mediator.Send(new ObtenerServiciosQuery([idServicio])); return StatusCode(result.StatusCode, result); }
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
