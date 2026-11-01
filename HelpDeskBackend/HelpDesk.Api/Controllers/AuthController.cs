using HelpDesk.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace HelpDesk.Api.Controllers;
[ApiController,Route("api/auth")]
public sealed class AuthController(IMediator mediator):ControllerBase
{
    [HttpPost("registro"),AllowAnonymous,EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegistroDto datos,CancellationToken ct)
    {var r=await mediator.Send(new RegistrarCommand(datos),ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("login"),AllowAnonymous,EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginDto datos,CancellationToken ct)
    {var r=await mediator.Send(new IniciarSesionCommand(datos),ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("cambiar-clave"),Authorize]
    public async Task<IActionResult> ChangePassword(CambioClaveDto datos,CancellationToken ct)
    {var r=await mediator.Send(new CambiarClaveCommand(datos),ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("recuperacion/{idUsuario:guid}"),Authorize]
    public async Task<IActionResult> IssueRecovery(Guid idUsuario,CancellationToken ct)
    {var r=await mediator.Send(new EmitirRecuperacionCommand(idUsuario),ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("restablecer-clave"),AllowAnonymous,EnableRateLimiting("auth")]
    public async Task<IActionResult> Reset(RestablecerClaveDto datos,CancellationToken ct)
    {var r=await mediator.Send(new RestablecerClaveCommand(datos),ct);return StatusCode(r.StatusCode,r);}
}
