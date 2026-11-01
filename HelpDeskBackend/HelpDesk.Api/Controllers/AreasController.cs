using HelpDesk.Application.UseCases.Areas;
using HelpDesk.Application.UseCases.Areas.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Areas"), Authorize]
public sealed class AreasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarAreasQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idArea:guid}")]
    public async Task<IActionResult> Get(Guid idArea, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerAreasQuery([idArea]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AreasInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearAreasCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idArea:guid}")]
    public async Task<IActionResult> Actualizar(Guid idArea, [FromBody] AreasInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarAreasCommand([idArea], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idArea:guid}")]
    public async Task<IActionResult> Eliminar(Guid idArea, CancellationToken ct)
    { var result = await mediator.Send(new EliminarAreasCommand([idArea]), ct); return StatusCode(result.StatusCode, result); }

}
