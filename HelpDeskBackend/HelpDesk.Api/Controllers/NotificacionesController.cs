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
    public async Task<IActionResult> List([FromQuery] ListarNotificacionesQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idNotificacion:guid}")]
    public async Task<IActionResult> Get(Guid idNotificacion)
    { var result = await mediator.Send(new ObtenerNotificacionesQuery { IdNotificacion = idNotificacion }); return StatusCode(result.StatusCode, result); }
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
