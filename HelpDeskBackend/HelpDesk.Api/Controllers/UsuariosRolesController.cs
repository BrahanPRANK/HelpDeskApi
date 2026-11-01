using HelpDesk.Application.UseCases.UsuariosRoles;
using HelpDesk.Application.UseCases.UsuariosRoles.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/UsuariosRoles"), Authorize]
public sealed class UsuariosRolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarUsuariosRolesQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idUsuario:guid}/{idRol:guid}")]
    public async Task<IActionResult> Get(Guid idUsuario, Guid idRol, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerUsuariosRolesQuery([idUsuario, idRol]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] UsuariosRolesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearUsuariosRolesCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idUsuario:guid}/{idRol:guid}")]
    public async Task<IActionResult> Actualizar(Guid idUsuario, Guid idRol, [FromBody] UsuariosRolesInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarUsuariosRolesCommand([idUsuario, idRol], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idUsuario:guid}/{idRol:guid}")]
    public async Task<IActionResult> Eliminar(Guid idUsuario, Guid idRol, CancellationToken ct)
    { var result = await mediator.Send(new EliminarUsuariosRolesCommand([idUsuario, idRol]), ct); return StatusCode(result.StatusCode, result); }

}
