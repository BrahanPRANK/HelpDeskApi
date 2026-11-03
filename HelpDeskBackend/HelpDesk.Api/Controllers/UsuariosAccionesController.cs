using HelpDesk.Domain.Dtos.Common;
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
    public async Task<IActionResult> ActivarUsuario(Guid id, ActivarUsuarioCommand datos)
    {
        datos.Id = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/desactivar")]
    public async Task<IActionResult> DesactivarUsuario(Guid id, DesactivarUsuarioCommand datos)
    {
        datos.Id = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

}
