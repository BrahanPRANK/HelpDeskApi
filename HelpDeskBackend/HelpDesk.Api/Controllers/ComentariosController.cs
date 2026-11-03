using HelpDesk.Application.UseCases.Comentarios;
using HelpDesk.Domain.Dtos.Comentarios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Comentarios"), Authorize]
public sealed class ComentariosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarComentariosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idComentario:guid}")]
    public async Task<IActionResult> Get(Guid idComentario)
    { var result = await mediator.Send(new ObtenerComentariosQuery { IdComentario = idComentario }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearComentariosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idComentario:guid}")]
    public async Task<IActionResult> Actualizar(Guid idComentario, [FromBody] ActualizarComentariosCommand datos)
    {
        datos.Keys = [idComentario];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
