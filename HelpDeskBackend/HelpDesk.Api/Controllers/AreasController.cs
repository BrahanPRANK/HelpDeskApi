using HelpDesk.Application.UseCases.Areas;
using HelpDesk.Application.Dtos.Areas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Areas"), Authorize]
public sealed class AreasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarAreasQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idArea:guid}")]
    public async Task<IActionResult> Get(Guid idArea)
    { var result = await mediator.Send(new ObtenerAreasQuery([idArea])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AreasInputDto datos)
    { var result = await mediator.Send(new CrearAreasCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idArea:guid}")]
    public async Task<IActionResult> Actualizar(Guid idArea, [FromBody] AreasInputDto datos)
    { var result = await mediator.Send(new ActualizarAreasCommand([idArea], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idArea:guid}")]
    public async Task<IActionResult> Eliminar(Guid idArea)
    { var result = await mediator.Send(new EliminarAreasCommand([idArea])); return StatusCode(result.StatusCode, result); }

}
