using HelpDesk.Application.UseCases.ArticulosConocimiento.Commands;
using HelpDesk.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/ArticulosConocimiento"), Authorize]
public sealed class ArticulosConocimientoAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/publicar")]
    public async Task<IActionResult> PublicarArticulo(Guid id, AccionDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new PublicarArticuloCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/archivar")]
    public async Task<IActionResult> ArchivarArticulo(Guid id, AccionDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new ArchivarArticuloCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

}
