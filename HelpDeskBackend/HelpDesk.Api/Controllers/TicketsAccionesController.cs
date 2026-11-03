using HelpDesk.Application.UseCases.Tickets.Commands;
using HelpDesk.Domain.Dtos.Tickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HelpDesk.Api.Controllers;

[ApiController, Route("api/Tickets"), Authorize]
public sealed class TicketsAccionesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{id:guid}/asignar")]
    public async Task<IActionResult> AsignarTicket(Guid id, AsignarTicketCommand datos)
    {
        datos.IdTicket = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/iniciar")]
    public async Task<IActionResult> IniciarTicket(Guid id, IniciarTicketCommand datos)
    {
        datos.IdTicket = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }





    [HttpPost("{id:guid}/resolver")]
    public async Task<IActionResult> ResolverTicket(Guid id, ResolverTicketCommand datos)
    {
        datos.IdTicket = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id:guid}/cerrar")]
    public async Task<IActionResult> CerrarTicket(Guid id, CerrarTicketCommand datos)
    {
        datos.IdTicket = id;

        var result = await mediator.Send(datos);
        return StatusCode(result.StatusCode, result);
    }





}
