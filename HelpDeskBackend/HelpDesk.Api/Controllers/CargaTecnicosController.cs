using HelpDesk.Application.UseCases.CargaTecnicos;
using HelpDesk.Domain.Dtos.CargaTecnicos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/CargaTecnicos"), Authorize]
public sealed class CargaTecnicosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ListarCargaTecnicosQuery query)
    {
        var result = await mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

}
