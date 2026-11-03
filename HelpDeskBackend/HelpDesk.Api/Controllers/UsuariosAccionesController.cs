using HelpDesk.Application.Dtos.Common;
using HelpDesk.Application.UseCases.Usuarios.Commands;
using HelpDesk.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Usuarios"), Authorize]
public sealed class UsuariosAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/activar")]
    public async Task<IActionResult> ActivarUsuario(Guid id, AccionDto datos)
    {
        var result = await mediator.Send(new ActivarUsuarioCommand(id, datos));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/desactivar")]
    public async Task<IActionResult> DesactivarUsuario(Guid id, AccionDto datos)
    {
        var result = await mediator.Send(new DesactivarUsuarioCommand(id, datos));
        return StatusCode(result.StatusCode, result);
    }

}
