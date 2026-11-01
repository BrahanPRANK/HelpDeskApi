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
    public async Task<IActionResult> MarcarNotificacionLeida(Guid id, AccionDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new MarcarNotificacionLeidaCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

}
