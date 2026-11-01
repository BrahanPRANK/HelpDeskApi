using HelpDesk.Application.UseCases.Notificaciones;
using HelpDesk.Application.UseCases.Notificaciones.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Notificaciones"), Authorize]
public sealed class NotificacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarNotificacionesQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idNotificacion:guid}")]
    public async Task<IActionResult> Get(Guid idNotificacion, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerNotificacionesQuery([idNotificacion]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] NotificacionesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearNotificacionesCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idNotificacion:guid}")]
    public async Task<IActionResult> Actualizar(Guid idNotificacion, [FromBody] NotificacionesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarNotificacionesCommand([idNotificacion], datos), ct); return StatusCode(result.StatusCode, result); }

}
