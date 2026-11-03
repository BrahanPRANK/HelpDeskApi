using HelpDesk.Application.UseCases.Notificaciones;
using HelpDesk.Domain.Dtos.Notificaciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Notificaciones"), Authorize]
public sealed class NotificacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarNotificacionesQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idNotificacion:guid}")]
    public async Task<IActionResult> Get(Guid idNotificacion)
    { var result = await mediator.Send(new ObtenerNotificacionesQuery([idNotificacion])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearNotificacionesCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idNotificacion:guid}")]
    public async Task<IActionResult> Actualizar(Guid idNotificacion, [FromBody] ActualizarNotificacionesCommand datos)
    {
        datos.Keys = [idNotificacion];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
