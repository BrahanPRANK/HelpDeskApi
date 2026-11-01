using HelpDesk.Application.UseCases.Sedes;
using HelpDesk.Application.UseCases.Sedes.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Sedes"), Authorize]
public sealed class SedesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarSedesQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idSede:guid}")]
    public async Task<IActionResult> Get(Guid idSede, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerSedesQuery([idSede]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] SedesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearSedesCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idSede:guid}")]
    public async Task<IActionResult> Actualizar(Guid idSede, [FromBody] SedesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarSedesCommand([idSede], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idSede:guid}")]
    public async Task<IActionResult> Eliminar(Guid idSede, CancellationToken ct)
    { var result = await mediator.Send(new EliminarSedesCommand([idSede]), ct); return StatusCode(result.StatusCode, result); }

}
