using HelpDesk.Application.UseCases.TareasCambios;
using HelpDesk.Application.UseCases.TareasCambios.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/TareasCambios"), Authorize]
public sealed class TareasCambiosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarTareasCambiosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idTareaCambio:guid}")]
    public async Task<IActionResult> Get(Guid idTareaCambio, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerTareasCambiosQuery([idTareaCambio]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] TareasCambiosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearTareasCambiosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idTareaCambio:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTareaCambio, [FromBody] TareasCambiosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarTareasCambiosCommand([idTareaCambio], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idTareaCambio:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTareaCambio, CancellationToken ct)
    { var result = await mediator.Send(new EliminarTareasCambiosCommand([idTareaCambio]), ct); return StatusCode(result.StatusCode, result); }

}
