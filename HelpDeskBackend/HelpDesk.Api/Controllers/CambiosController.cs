using HelpDesk.Application.UseCases.Cambios;
using HelpDesk.Domain.Dtos.Cambios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Cambios"), Authorize]
public sealed class CambiosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarCambiosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idCambio:guid}")]
    public async Task<IActionResult> Get(Guid idCambio)
    { var result = await mediator.Send(new ObtenerCambiosQuery { IdCambio = idCambio }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCambiosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idCambio:guid}")]
    public async Task<IActionResult> Actualizar(Guid idCambio, [FromBody] ActualizarCambiosCommand datos)
    {
        datos.Keys = [idCambio];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
