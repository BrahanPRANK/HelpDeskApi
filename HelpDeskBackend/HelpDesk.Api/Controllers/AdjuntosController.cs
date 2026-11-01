using HelpDesk.Application.UseCases.Adjuntos;
using HelpDesk.Application.UseCases.Adjuntos.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Adjuntos"), Authorize]
public sealed class AdjuntosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarAdjuntosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAdjunto:guid}")]
    public async Task<IActionResult> Get(Guid idAdjunto, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerAdjuntosQuery([idAdjunto]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AdjuntosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearAdjuntosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idAdjunto:guid}")]
    public async Task<IActionResult> Actualizar(Guid idAdjunto, [FromBody] AdjuntosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarAdjuntosCommand([idAdjunto], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idAdjunto:guid}")]
    public async Task<IActionResult> Eliminar(Guid idAdjunto, CancellationToken ct)
    { var result = await mediator.Send(new EliminarAdjuntosCommand([idAdjunto]), ct); return StatusCode(result.StatusCode, result); }

}
