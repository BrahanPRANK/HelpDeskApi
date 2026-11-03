using HelpDesk.Application.UseCases.Problemas;
using HelpDesk.Domain.Dtos.Problemas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Problemas"), Authorize]
public sealed class ProblemasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pagina = 1, [FromQuery] int tamano = 20, [FromQuery] string? buscar = null, [FromQuery] string? estado = null, [FromQuery] Guid? idTicket = null)
    { var result = await mediator.Send(new ListarProblemasQuery(new(pagina, tamano, buscar, estado, idTicket))); return StatusCode(result.StatusCode, result); }
    [HttpGet("{idProblema:guid}")]
    public async Task<IActionResult> Get(Guid idProblema)
    { var result = await mediator.Send(new ObtenerProblemasQuery([idProblema])); return StatusCode(result.StatusCode, result); }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearProblemasCommand datos)
    {
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{idProblema:guid}")]
    public async Task<IActionResult> Actualizar(Guid idProblema, [FromBody] ActualizarProblemasCommand datos)
    {
        datos.Keys = [idProblema];
        var result = await mediator.Send(datos); return StatusCode(result.StatusCode, result);
    }

}
