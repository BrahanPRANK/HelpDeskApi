using HelpDesk.Application.UseCases.ArticulosConocimiento;
using HelpDesk.Application.UseCases.ArticulosConocimiento.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/ArticulosConocimiento"), Authorize]
public sealed class ArticulosConocimientoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarArticulosConocimientoQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idArticulo:guid}")]
    public async Task<IActionResult> Get(Guid idArticulo, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerArticulosConocimientoQuery([idArticulo]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ArticulosConocimientoInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearArticulosConocimientoCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idArticulo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idArticulo, [FromBody] ArticulosConocimientoInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarArticulosConocimientoCommand([idArticulo], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idArticulo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idArticulo, CancellationToken ct)
    { var result = await mediator.Send(new EliminarArticulosConocimientoCommand([idArticulo]), ct); return StatusCode(result.StatusCode, result); }

}
