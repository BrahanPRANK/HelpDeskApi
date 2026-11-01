using HelpDesk.Application.UseCases.Servicios;
using HelpDesk.Application.UseCases.Servicios.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;
[ApiController, Route("api/Servicios"), Authorize]
public sealed class ServiciosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null, CancellationToken ct = default)
    { var result = await mediator.Send(new ListarServiciosQuery(new(pagina, tamano, buscar, estado, idTicket)), ct); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idServicio:guid}")]
    public async Task<IActionResult> Get(Guid idServicio, CancellationToken ct)
    { var result = await mediator.Send(new ObtenerServiciosQuery([idServicio]), ct); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ServiciosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new CrearServiciosCommand(datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idServicio:guid}")]
    public async Task<IActionResult> Actualizar(Guid idServicio, [FromBody] ServiciosInputDto datos, CancellationToken ct)
    { var result = await mediator.Send(new ActualizarServiciosCommand([idServicio], datos), ct); return StatusCode(result.StatusCode, result); }
    [HttpDelete("{idServicio:guid}")]
    public async Task<IActionResult> Eliminar(Guid idServicio, CancellationToken ct)
    { var result = await mediator.Send(new EliminarServiciosCommand([idServicio]), ct); return StatusCode(result.StatusCode, result); }

}
