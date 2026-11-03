using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.UseCases.Notificaciones.Commands;
using HelpDesk.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Notificaciones"), Authorize]
public sealed class NotificacionesAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/marcar-leida")]
    public async Task<IActionResult> MarcarNotificacionLeida(Guid id, MarcarNotificacionLeidaCommand datos)
    {
        datos.Id = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

}
