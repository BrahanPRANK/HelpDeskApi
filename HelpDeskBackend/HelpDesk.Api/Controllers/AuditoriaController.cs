using HelpDesk.Application.UseCases.Auditoria;
using HelpDesk.Domain.Dtos.Auditoria;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Auditoria"), Authorize]
public sealed class AuditoriaController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarAuditoriaQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idAuditoria:guid}")]
    public async Task<IActionResult> Get(Guid idAuditoria)
    { var result = await mediator.Send(new ObtenerAuditoriaQuery { IdAuditoria = idAuditoria }); return StatusCode(result.StatusCode, result); }

}
