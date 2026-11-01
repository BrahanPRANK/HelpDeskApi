using HelpDesk.Application.UseCases.Aprobaciones;
using HelpDesk.Application.UseCases.Aprobaciones.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Aprobaciones"), Authorize]
public sealed class AprobacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarAprobacionesQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAprobacion:guid}")]
    public async Task<IActionResult> Get(Guid idAprobacion, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerAprobacionesQuery([idAprobacion]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AprobacionesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearAprobacionesCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idAprobacion:guid}")]
    public async Task<IActionResult> Actualizar(Guid idAprobacion, [FromBody] AprobacionesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarAprobacionesCommand([idAprobacion], datos), ct); return StatusCode(result.StatusCode, result); }

}
