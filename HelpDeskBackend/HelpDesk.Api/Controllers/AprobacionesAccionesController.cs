using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.UseCases.Aprobaciones.Commands;
using HelpDesk.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Aprobaciones"), Authorize]
public sealed class AprobacionesAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/aprobar")]
    public async Task<IActionResult> AprobarSolicitud(Guid id, AprobarSolicitudCommand datos)
    {
        datos.Id = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/rechazar")]
    public async Task<IActionResult> RechazarSolicitud(Guid id, RechazarSolicitudCommand datos)
    {
        datos.Id = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

}
