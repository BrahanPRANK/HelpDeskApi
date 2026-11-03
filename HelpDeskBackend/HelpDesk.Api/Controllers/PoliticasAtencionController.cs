using HelpDesk.Application.UseCases.PoliticasAtencion;
using HelpDesk.Domain.Dtos.PoliticasAtencion;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/PoliticasAtencion"), Authorize]
public sealed class PoliticasAtencionController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarPoliticasAtencionQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idPolitica:guid}")]
    public async Task<IActionResult> Get(Guid idPolitica)
    { var result = await mediator.Send(new ObtenerPoliticasAtencionQuery([idPolitica])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] PoliticasAtencionInputDto datos)
    { var result = await mediator.Send(new CrearPoliticasAtencionCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idPolitica:guid}")]
    public async Task<IActionResult> Actualizar(Guid idPolitica, [FromBody] PoliticasAtencionInputDto datos)
    { var result = await mediator.Send(new ActualizarPoliticasAtencionCommand([idPolitica], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idPolitica:guid}")]
    public async Task<IActionResult> Eliminar(Guid idPolitica)
    { var result = await mediator.Send(new EliminarPoliticasAtencionCommand([idPolitica])); return StatusCode(result.StatusCode, result); }

}
