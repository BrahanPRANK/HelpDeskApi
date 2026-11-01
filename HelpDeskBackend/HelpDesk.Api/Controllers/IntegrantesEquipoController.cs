using HelpDesk.Application.UseCases.IntegrantesEquipo;
using HelpDesk.Application.UseCases.IntegrantesEquipo.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/IntegrantesEquipo"), Authorize]
public sealed class IntegrantesEquipoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarIntegrantesEquipoQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Get(Guid idEquipo, Guid idUsuario, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerIntegrantesEquipoQuery([idEquipo, idUsuario]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] IntegrantesEquipoInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearIntegrantesEquipoCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idEquipo, Guid idUsuario, [FromBody] IntegrantesEquipoInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarIntegrantesEquipoCommand([idEquipo, idUsuario], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Eliminar(Guid idEquipo, Guid idUsuario, CancellationToken ct)
    { var result = await mediator.Send(new EliminarIntegrantesEquipoCommand([idEquipo, idUsuario]), ct); return StatusCode(result.StatusCode, result); }

}
