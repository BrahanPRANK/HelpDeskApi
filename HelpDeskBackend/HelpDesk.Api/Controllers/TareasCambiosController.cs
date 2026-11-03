using HelpDesk.Application.UseCases.TareasCambios;
using HelpDesk.Domain.Dtos.TareasCambios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/TareasCambios"), Authorize]
public sealed class TareasCambiosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarTareasCambiosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idTareaCambio:guid}")]
    public async Task<IActionResult> Get(Guid idTareaCambio)
    { var result = await mediator.Send(new ObtenerTareasCambiosQuery([idTareaCambio])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTareasCambiosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idTareaCambio:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTareaCambio, [FromBody] ActualizarTareasCambiosCommand datos)
    {
        datos.Keys = [idTareaCambio];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idTareaCambio:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTareaCambio)
    { var result = await mediator.Send(new EliminarTareasCambiosCommand { Keys = [idTareaCambio] }); return StatusCode(result.StatusCode, result); }

}
