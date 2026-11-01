using HelpDesk.Application.UseCases.Tickets.Commands;
using HelpDesk.Application.UseCases.Tickets.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Tickets"), Authorize]
public sealed class TicketsAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/asignar")]
    public async Task<IActionResult> AsignarTicket(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new AsignarTicketCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/iniciar")]
    public async Task<IActionResult> IniciarTicket(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new IniciarTicketCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/esperar")]
    public async Task<IActionResult> PonerTicketEnEspera(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new PonerTicketEnEsperaCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/reanudar")]
    public async Task<IActionResult> ReanudarTicket(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new ReanudarTicketCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/resolver")]
    public async Task<IActionResult> ResolverTicket(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new ResolverTicketCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/cerrar")]
    public async Task<IActionResult> CerrarTicket(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new CerrarTicketCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/reabrir")]
    public async Task<IActionResult> ReabrirTicket(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new ReabrirTicketCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/cancelar")]
    public async Task<IActionResult> CancelarTicket(Guid id, AccionTicketDto datos, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelarTicketCommand(id, datos), ct);
        return StatusCode(result.StatusCode, result);
    }

}
