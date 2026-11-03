using HelpDesk.Application.UseCases.Reportes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Reportes"), Authorize]
public sealed class ReportesController(IMediator mediator) : ControllerBase
{
    [HttpGet("indicadores")]
    public async Task<IActionResult> Indicadores()
    {
        var result = await mediator.Send(new IndicadoresQuery());
        return StatusCode(result.StatusCode, result);
    }
}
