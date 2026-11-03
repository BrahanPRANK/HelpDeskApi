using HelpDesk.Application.UseCases.Adjuntos;
using HelpDesk.Application.Dtos.Adjuntos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Adjuntos"), Authorize]
public sealed class AdjuntosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarAdjuntosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAdjunto:guid}")]
    public async Task<IActionResult> Get(Guid idAdjunto)
    { var result = await mediator.Send(new ObtenerAdjuntosQuery([idAdjunto])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AdjuntosInputDto datos)
    { var result = await mediator.Send(new CrearAdjuntosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idAdjunto:guid}")]
    public async Task<IActionResult> Actualizar(Guid idAdjunto, [FromBody] AdjuntosInputDto datos)
    { var result = await mediator.Send(new ActualizarAdjuntosCommand([idAdjunto], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idAdjunto:guid}")]
    public async Task<IActionResult> Eliminar(Guid idAdjunto)
    { var result = await mediator.Send(new EliminarAdjuntosCommand([idAdjunto])); return StatusCode(result.StatusCode, result); }

}
