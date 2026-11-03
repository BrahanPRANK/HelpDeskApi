using HelpDesk.Application.UseCases.CategoriasConocimiento;
using HelpDesk.Domain.Dtos.CategoriasConocimiento;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/CategoriasConocimiento"), Authorize]
public sealed class CategoriasConocimientoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarCategoriasConocimientoQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idCategoriaConocimiento:guid}")]
    public async Task<IActionResult> Get(Guid idCategoriaConocimiento)
    { var result = await mediator.Send(new ObtenerCategoriasConocimientoQuery([idCategoriaConocimiento])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CategoriasConocimientoInputDto datos)
    { var result = await mediator.Send(new CrearCategoriasConocimientoCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idCategoriaConocimiento:guid}")]
    public async Task<IActionResult> Actualizar(Guid idCategoriaConocimiento, [FromBody] CategoriasConocimientoInputDto datos)
    { var result = await mediator.Send(new ActualizarCategoriasConocimientoCommand([idCategoriaConocimiento], datos)); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idCategoriaConocimiento:guid}")]
    public async Task<IActionResult> Eliminar(Guid idCategoriaConocimiento)
    { var result = await mediator.Send(new EliminarCategoriasConocimientoCommand([idCategoriaConocimiento])); return StatusCode(result.StatusCode, result); }

}
