using HelpDesk.Application.UseCases.RecuperacionesClave;
using HelpDesk.Application.UseCases.RecuperacionesClave.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/RecuperacionesClave"), Authorize]
public sealed class RecuperacionesClaveController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarRecuperacionesClaveQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idRecuperacion:guid}")]
    public async Task<IActionResult> Get(Guid idRecuperacion, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerRecuperacionesClaveQuery([idRecuperacion]), ct); return StatusCode(result.StatusCode, result); }

}
