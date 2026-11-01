using HelpDesk.Application.UseCases.Auditoria;
using HelpDesk.Application.UseCases.Auditoria.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Auditoria"), Authorize]
public sealed class AuditoriaController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarAuditoriaQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAuditoria:guid}")]
    public async Task<IActionResult> Get(Guid idAuditoria, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerAuditoriaQuery([idAuditoria]), ct); return StatusCode(result.StatusCode, result); }

}
