using HelpDesk.Application.UseCases.Permisos;
using HelpDesk.Domain.Dtos.Permisos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Permisos"), Authorize]
public sealed class PermisosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarPermisosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idPermiso:guid}")]
    public async Task<IActionResult> Get(Guid idPermiso)
    { var result = await mediator.Send(new ObtenerPermisosQuery([idPermiso])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] PermisosInputDto datos)
    { var result = await mediator.Send(new CrearPermisosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idPermiso:guid}")]
    public async Task<IActionResult> Actualizar(Guid idPermiso, [FromBody] PermisosInputDto datos)
    { var result = await mediator.Send(new ActualizarPermisosCommand([idPermiso], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idPermiso:guid}")]
    public async Task<IActionResult> Eliminar(Guid idPermiso)
    { var result = await mediator.Send(new EliminarPermisosCommand([idPermiso])); return StatusCode(result.StatusCode, result); }

}
