using HelpDesk.Application.UseCases.TiposActivos;
using HelpDesk.Domain.Dtos.TiposActivos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/TiposActivos"), Authorize]
public sealed class TiposActivosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarTiposActivosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idTipoActivo:guid}")]
    public async Task<IActionResult> Get(Guid idTipoActivo)
    { var result = await mediator.Send(new ObtenerTiposActivosQuery([idTipoActivo])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] TiposActivosInputDto datos)
    { var result = await mediator.Send(new CrearTiposActivosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idTipoActivo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idTipoActivo, [FromBody] TiposActivosInputDto datos)
    { var result = await mediator.Send(new ActualizarTiposActivosCommand([idTipoActivo], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idTipoActivo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idTipoActivo)
    { var result = await mediator.Send(new EliminarTiposActivosCommand([idTipoActivo])); return StatusCode(result.StatusCode, result); }

}
