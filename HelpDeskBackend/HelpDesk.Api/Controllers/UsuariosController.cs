using HelpDesk.Application.UseCases.Usuarios;
using HelpDesk.Application.UseCases.Usuarios.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Usuarios"), Authorize]
public sealed class UsuariosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarUsuariosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idUsuario:guid}")]
    public async Task<IActionResult> Get(Guid idUsuario, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerUsuariosQuery([idUsuario]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] UsuariosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearUsuariosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idUsuario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idUsuario, [FromBody] UsuariosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarUsuariosCommand([idUsuario], datos), ct); return StatusCode(result.StatusCode, result); }

}
