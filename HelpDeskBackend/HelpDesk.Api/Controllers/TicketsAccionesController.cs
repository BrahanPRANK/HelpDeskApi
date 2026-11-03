using HelpDesk.Application.UseCases.Tickets.Commands;
using HelpDesk.Application.Dtos.Tickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Tickets"), Authorize]
public sealed class TicketsAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/asignar")]
    public async Task<IActionResult> AsignarTicket(Guid id, AccionTicketDto datos)
    {
        var result = await mediator.Send(new AsignarTicketCommand(id, datos));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/iniciar")]
    public async Task<IActionResult> IniciarTicket(Guid id, AccionTicketDto datos)
    {
        var result = await mediator.Send(new IniciarTicketCommand(id, datos));
        return StatusCode(result.StatusCode, result);
    }





    [HttpPost("{id:guid}/resolver")]
    public async Task<IActionResult> ResolverTicket(Guid id, AccionTicketDto datos)
    {
        var result = await mediator.Send(new ResolverTicketCommand(id, datos));
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/cerrar")]
    public async Task<IActionResult> CerrarTicket(Guid id, AccionTicketDto datos)
    {
        var result = await mediator.Send(new CerrarTicketCommand(id, datos));
        return StatusCode(result.StatusCode, result);
    }





}
