using HelpDesk.Application.UseCases.Activos;
using HelpDesk.Application.UseCases.Activos.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Activos"), Authorize]
public sealed class ActivosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarActivosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idActivo:guid}")]
    public async Task<IActionResult> Get(Guid idActivo, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerActivosQuery([idActivo]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ActivosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearActivosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idActivo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idActivo, [FromBody] ActivosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarActivosCommand([idActivo], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idActivo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idActivo, CancellationToken ct)
    { var result = await mediator.Send(new EliminarActivosCommand([idActivo]), ct); return StatusCode(result.StatusCode, result); }

}
