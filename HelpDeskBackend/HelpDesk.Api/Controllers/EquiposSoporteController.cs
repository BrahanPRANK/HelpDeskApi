using HelpDesk.Application.UseCases.EquiposSoporte;
using HelpDesk.Domain.Dtos.EquiposSoporte;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/EquiposSoporte"), Authorize]
public sealed class EquiposSoporteController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarEquiposSoporteQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idEquipo:guid}")]
    public async Task<IActionResult> Get(Guid idEquipo)
    { var result = await mediator.Send(new ObtenerEquiposSoporteQuery([idEquipo])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEquiposSoporteCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idEquipo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idEquipo, [FromBody] ActualizarEquiposSoporteCommand datos)
    {
        datos.Keys = [idEquipo];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idEquipo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idEquipo)
    { var result = await mediator.Send(new EliminarEquiposSoporteCommand { Keys = [idEquipo] }); return StatusCode(result.StatusCode, result); }

}
