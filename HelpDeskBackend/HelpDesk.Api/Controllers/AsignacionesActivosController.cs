using HelpDesk.Application.UseCases.AsignacionesActivos;
using HelpDesk.Application.UseCases.AsignacionesActivos.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/AsignacionesActivos"), Authorize]
public sealed class AsignacionesActivosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarAsignacionesActivosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAsignacion:guid}")]
    public async Task<IActionResult> Get(Guid idAsignacion, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerAsignacionesActivosQuery([idAsignacion]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AsignacionesActivosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearAsignacionesActivosCommand(datos), ct); return StatusCode(result.StatusCode, result); }

}
