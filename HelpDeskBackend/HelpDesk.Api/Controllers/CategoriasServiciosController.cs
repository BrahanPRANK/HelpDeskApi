using HelpDesk.Application.UseCases.CategoriasServicios;
using HelpDesk.Domain.Dtos.CategoriasServicios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/CategoriasServicios"), Authorize]
public sealed class CategoriasServiciosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarCategoriasServiciosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{idCategoriaServicio:guid}")]
    public async Task<IActionResult> Get(Guid idCategoriaServicio)
    { var result = await mediator.Send(new ObtenerCategoriasServiciosQuery { IdCategoriaServicio = idCategoriaServicio }); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriasServiciosCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idCategoriaServicio:guid}")]
    public async Task<IActionResult> Actualizar(Guid idCategoriaServicio, [FromBody] ActualizarCategoriasServiciosCommand datos)
    {
        datos.Keys = [idCategoriaServicio];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpDelete("{idCategoriaServicio:guid}")]
    public async Task<IActionResult> Eliminar(Guid idCategoriaServicio)
    { var result = await mediator.Send(new EliminarCategoriasServiciosCommand { Keys = [idCategoriaServicio] }); return StatusCode(result.StatusCode, result); }

}
