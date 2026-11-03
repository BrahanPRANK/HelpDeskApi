using HelpDesk.Application.UseCases.PreferenciasNotificaciones;
using HelpDesk.Domain.Dtos.PreferenciasNotificaciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/PreferenciasNotificaciones"), Authorize]
public sealed class PreferenciasNotificacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarPreferenciasNotificacionesQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idUsuario:guid}/{tipoEvento}")]
    public async Task<IActionResult> Get(Guid idUsuario, string tipoEvento)
    { var result = await mediator.Send(new ObtenerPreferenciasNotificacionesQuery { IdUsuario = idUsuario, TipoEvento = tipoEvento }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPreferenciasNotificacionesCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idUsuario:guid}/{tipoEvento}")]
    public async Task<IActionResult> Actualizar(Guid idUsuario, string tipoEvento, [FromBody] ActualizarPreferenciasNotificacionesCommand datos)
    {
        datos.Keys = [idUsuario, tipoEvento];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idUsuario:guid}/{tipoEvento}")]
    public async Task<IActionResult> Eliminar(Guid idUsuario, string tipoEvento)
    { var result = await mediator.Send(new EliminarPreferenciasNotificacionesCommand { Keys = [idUsuario, tipoEvento] }); return StatusCode(result.StatusCode, result); }

}
