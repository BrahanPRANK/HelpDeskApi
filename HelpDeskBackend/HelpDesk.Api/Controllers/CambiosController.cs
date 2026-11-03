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
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarCambiosQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idCambio:guid}")]
    public async Task<IActionResult> Get(Guid idCambio)
    { var result = await mediator.Send(new ObtenerCambiosQuery([idCambio])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CambiosInputDto datos)
    { var result = await mediator.Send(new CrearCambiosCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idCambio:guid}")]
    public async Task<IActionResult> Actualizar(Guid idCambio, [FromBody] CambiosInputDto datos)
    { var result = await mediator.Send(new ActualizarCambiosCommand([idCambio], datos)); return StatusCode(result.StatusCode, result); }

}
