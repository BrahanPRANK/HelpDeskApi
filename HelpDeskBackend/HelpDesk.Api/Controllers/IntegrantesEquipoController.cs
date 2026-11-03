using HelpDesk.Application.UseCases.IntegrantesEquipo;
using HelpDesk.Application.Dtos.IntegrantesEquipo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/IntegrantesEquipo"), Authorize]
public sealed class IntegrantesEquipoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarIntegrantesEquipoQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Get(Guid idEquipo, Guid idUsuario)
    { var result = await mediator.Send(new ObtenerIntegrantesEquipoQuery([idEquipo, idUsuario])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] IntegrantesEquipoInputDto datos)
    { var result = await mediator.Send(new CrearIntegrantesEquipoCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idEquipo, Guid idUsuario, [FromBody] IntegrantesEquipoInputDto datos)
    { var result = await mediator.Send(new ActualizarIntegrantesEquipoCommand([idEquipo, idUsuario], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idEquipo:guid}/{idUsuario:guid}")]
    public async Task<IActionResult> Eliminar(Guid idEquipo, Guid idUsuario)
    { var result = await mediator.Send(new EliminarIntegrantesEquipoCommand([idEquipo, idUsuario])); return StatusCode(result.StatusCode, result); }

}
