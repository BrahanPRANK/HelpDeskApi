using HelpDesk.Application.UseCases.Comentarios;
using HelpDesk.Application.UseCases.Comentarios.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Comentarios"), Authorize]
public sealed class ComentariosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarComentariosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idComentario:guid}")]
    public async Task<IActionResult> Get(Guid idComentario, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerComentariosQuery([idComentario]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ComentariosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearComentariosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idComentario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idComentario, [FromBody] ComentariosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarComentariosCommand([idComentario], datos), ct); return StatusCode(result.StatusCode, result); }

}
