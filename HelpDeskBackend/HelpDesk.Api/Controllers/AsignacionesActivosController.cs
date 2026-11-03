using HelpDesk.Application.UseCases.AsignacionesActivos;
using HelpDesk.Domain.Dtos.AsignacionesActivos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/AsignacionesActivos"), Authorize]
public sealed class AsignacionesActivosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarAsignacionesActivosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAsignacion:guid}")]
    public async Task<IActionResult> Get(Guid idAsignacion)
    { var result = await mediator.Send(new ObtenerAsignacionesActivosQuery([idAsignacion])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AsignacionesActivosInputDto datos)
    { var result = await mediator.Send(new CrearAsignacionesActivosCommand(datos)); return StatusCode(result.StatusCode, result); }

}
