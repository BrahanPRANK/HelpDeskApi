using HelpDesk.Application.UseCases.Aprobaciones;
using HelpDesk.Application.Dtos.Aprobaciones;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Aprobaciones"), Authorize]
public sealed class AprobacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarAprobacionesQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idAprobacion:guid}")]
    public async Task<IActionResult> Get(Guid idAprobacion)
    { var result = await mediator.Send(new ObtenerAprobacionesQuery([idAprobacion])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AprobacionesInputDto datos)
    { var result = await mediator.Send(new CrearAprobacionesCommand(datos)); return StatusCode(result.StatusCode, result); }
    [HttpPut("{idAprobacion:guid}")]
    public async Task<IActionResult> Actualizar(Guid idAprobacion, [FromBody] AprobacionesInputDto datos)
    { var result = await mediator.Send(new ActualizarAprobacionesCommand([idAprobacion], datos)); return StatusCode(result.StatusCode, result); }

}
