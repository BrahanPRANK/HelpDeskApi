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
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarAuditoriaQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAuditoria:guid}")]
    public async Task<IActionResult> Get(Guid idAuditoria)
    { var result = await mediator.Send(new ObtenerAuditoriaQuery([idAuditoria])); return StatusCode(result.StatusCode, result); }

}
