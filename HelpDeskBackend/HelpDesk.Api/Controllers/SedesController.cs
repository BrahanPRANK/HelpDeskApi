using HelpDesk.Application.UseCases.Sedes;
using HelpDesk.Domain.Dtos.Sedes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Sedes"), Authorize]
public sealed class SedesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarSedesQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idSede:guid}")]
    public async Task<IActionResult> Get(Guid idSede)
    { var result = await mediator.Send(new ObtenerSedesQuery([idSede])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSedesCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idSede:guid}")]
    public async Task<IActionResult> Actualizar(Guid idSede, [FromBody] ActualizarSedesCommand datos)
    {
        datos.Keys = [idSede];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idSede:guid}")]
    public async Task<IActionResult> Eliminar(Guid idSede)
    { var result = await mediator.Send(new EliminarSedesCommand { Keys = [idSede] }); return StatusCode(result.StatusCode, result); }

}
