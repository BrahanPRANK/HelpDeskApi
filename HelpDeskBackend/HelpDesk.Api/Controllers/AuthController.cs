using HelpDesk.Application.Dtos.Autenticacion;
using HelpDesk.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("registro"), AllowAnonymous, EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegistroDto datos)
    { var r = await mediator.Send(new RegistrarCommand(datos)); return StatusCode(r.StatusCode, r); }
    [HttpPost("login"), AllowAnonymous, EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginDto datos)
    { var r = await mediator.Send(new IniciarSesionCommand(datos)); return StatusCode(r.StatusCode, r); }
    [HttpPost("cambiar-clave"), Authorize]
    public async Task<IActionResult> ChangePassword(CambioClaveDto datos)
    { var r = await mediator.Send(new CambiarClaveCommand(datos)); return StatusCode(r.StatusCode, r); }


}
