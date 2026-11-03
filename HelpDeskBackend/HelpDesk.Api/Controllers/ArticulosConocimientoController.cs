using HelpDesk.Application.UseCases.ArticulosConocimiento;
using HelpDesk.Domain.Dtos.ArticulosConocimiento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/ArticulosConocimiento"), Authorize]
public sealed class ArticulosConocimientoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarArticulosConocimientoQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idArticulo:guid}")]
    public async Task<IActionResult> Get(Guid idArticulo)
    { var result = await mediator.Send(new ObtenerArticulosConocimientoQuery([idArticulo])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ArticulosConocimientoInputDto datos)
    { var result = await mediator.Send(new CrearArticulosConocimientoCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idArticulo:guid}")]
    public async Task<IActionResult> Actualizar(Guid idArticulo, [FromBody] ArticulosConocimientoInputDto datos)
    { var result = await mediator.Send(new ActualizarArticulosConocimientoCommand([idArticulo], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idArticulo:guid}")]
    public async Task<IActionResult> Eliminar(Guid idArticulo)
    { var result = await mediator.Send(new EliminarArticulosConocimientoCommand([idArticulo])); return StatusCode(result.StatusCode, result); }

}
