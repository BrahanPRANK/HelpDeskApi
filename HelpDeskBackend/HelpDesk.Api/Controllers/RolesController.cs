using HelpDesk.Application.UseCases.Roles;
using HelpDesk.Domain.Dtos.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Roles"), Authorize]
public sealed class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarRolesQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idRol:guid}")]
    public async Task<IActionResult> Get(Guid idRol)
    { var result = await mediator.Send(new ObtenerRolesQuery([idRol])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearRolesCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idRol:guid}")]
    public async Task<IActionResult> Actualizar(Guid idRol, [FromBody] ActualizarRolesCommand datos)
    {
        datos.Keys = [idRol];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idRol:guid}")]
    public async Task<IActionResult> Eliminar(Guid idRol)
    { var result = await mediator.Send(new EliminarRolesCommand { Keys = [idRol] }); return StatusCode(result.StatusCode, result); }

}
