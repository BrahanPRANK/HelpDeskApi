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
    public async Task<IActionResult> List([FromQuery] ListarUsuariosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idUsuario:guid}")]
    public async Task<IActionResult> Get(Guid idUsuario)
    { var result = await mediator.Send(new ObtenerUsuariosQuery { IdUsuario = idUsuario }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuariosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idUsuario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idUsuario, [FromBody] ActualizarUsuariosCommand datos)
    {
        datos.Keys = [idUsuario];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
