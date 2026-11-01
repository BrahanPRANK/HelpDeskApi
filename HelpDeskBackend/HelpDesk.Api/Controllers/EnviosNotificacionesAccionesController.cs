using HelpDesk.Application.UseCases.EnviosNotificaciones.Commands;
using HelpDesk.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/EnviosNotificaciones"), Authorize]
public sealed class EnviosNotificacionesAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/reintentar")]
    public async Task<IActionResult> ReintentarEnvio(Guid id, AccionDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new ReintentarEnvioCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

}
