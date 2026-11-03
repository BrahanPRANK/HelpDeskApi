using HelpDesk.Application.UseCases.Usuarios;
using HelpDesk.Domain.Dtos.Usuarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Usuarios"), Authorize]
public sealed class UsuariosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarUsuariosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idUsuario:guid}")]
    public async Task<IActionResult> Get(Guid idUsuario)
    { var result = await mediator.Send(new ObtenerUsuariosQuery([idUsuario])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] UsuariosInputDto datos)
    { var result = await mediator.Send(new CrearUsuariosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idUsuario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idUsuario, [FromBody] UsuariosInputDto datos)
    { var result = await mediator.Send(new ActualizarUsuariosCommand([idUsuario], datos)); return StatusCode(result.StatusCode, result); }

}
