using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.UseCases.AsignacionesActivos.Commands;
using HelpDesk.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/AsignacionesActivos"), Authorize]
public sealed class AsignacionesActivosAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/devolver")]
    public async Task<IActionResult> DevolverActivo(Guid id, DevolverActivoCommand datos)
    {
        datos.Id = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

}
