using HelpDesk.Application.UseCases.RecuperacionesClave;
using HelpDesk.Domain.Dtos.RecuperacionesClave;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/RecuperacionesClave"), Authorize]
public sealed class RecuperacionesClaveController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarRecuperacionesClaveQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idRecuperacion:guid}")]
    public async Task<IActionResult> Get(Guid idRecuperacion)
    { var result = await mediator.Send(new ObtenerRecuperacionesClaveQuery { IdRecuperacion = idRecuperacion }); return StatusCode(result.StatusCode, result); }

}
