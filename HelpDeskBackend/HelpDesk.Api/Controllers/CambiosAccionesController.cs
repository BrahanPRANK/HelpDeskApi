using HelpDesk.Application.UseCases.Cambios.Commands;
using HelpDesk.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Cambios"), Authorize]
public sealed class CambiosAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/programar")]
    public async Task<IActionResult> ProgramarCambio(Guid id, AccionDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new ProgramarCambioCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/ejecutar")]
    public async Task<IActionResult> EjecutarCambio(Guid id, AccionDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new EjecutarCambioCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/finalizar")]
    public async Task<IActionResult> FinalizarCambio(Guid id, AccionDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new FinalizarCambioCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

}
