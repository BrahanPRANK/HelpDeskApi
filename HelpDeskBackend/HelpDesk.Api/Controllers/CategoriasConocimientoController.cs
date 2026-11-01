using HelpDesk.Application.UseCases.CategoriasConocimiento;
using HelpDesk.Application.UseCases.CategoriasConocimiento.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/CategoriasConocimiento"), Authorize]
public sealed class CategoriasConocimientoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarCategoriasConocimientoQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idCategoriaConocimiento:guid}")]
    public async Task<IActionResult> Get(Guid idCategoriaConocimiento, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerCategoriasConocimientoQuery([idCategoriaConocimiento]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CategoriasConocimientoInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearCategoriasConocimientoCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idCategoriaConocimiento:guid}")]
    public async Task<IActionResult> Actualizar(Guid idCategoriaConocimiento, [FromBody] CategoriasConocimientoInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarCategoriasConocimientoCommand([idCategoriaConocimiento], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idCategoriaConocimiento:guid}")]
    public async Task<IActionResult> Eliminar(Guid idCategoriaConocimiento, CancellationToken ct)
    { var result = await mediator.Send(new EliminarCategoriasConocimientoCommand([idCategoriaConocimiento]), ct); return StatusCode(result.StatusCode, result); }

}
