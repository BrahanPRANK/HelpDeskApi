using System.ComponentModel.DataAnnotations;
using HelpDesk.Domain.Dtos.Common;
using HelpDesk.Application.Common;
using HelpDesk.Application.Security;
using HelpDesk.Domain.Dtos.TicketsArticulos;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.Models;
using HelpDesk.Domain.Ports;
using MediatR;
namespace HelpDesk.Application.UseCases.TicketsArticulos;

public class CrearTicketsArticulosCommand : IRequest<ResponseDto<TicketsArticulosResponseDto>>
{
    public Guid IdTicket { get; set; }
    public Guid IdArticulo { get; set; }
    public Guid IdUsuario { get; set; }
}

internal sealed class CrearTicketsArticulosCommandHandler(IUnitOfWork _unitOfWork, AccessRules access, ICurrentUser actor) : IRequestHandler<CrearTicketsArticulosCommand, ResponseDto<TicketsArticulosResponseDto>>
{
    public async Task<ResponseDto<TicketsArticulosResponseDto>> Handle(CrearTicketsArticulosCommand request, CancellationToken ct)
    {
        try
        {

            Validation.Check(request);
            Validation.Require(access.Admin, "Esta operacion requiere administracion.", 403);
            var entity = new global::HelpDesk.Domain.Models.TicketsArticulos()
            {
                IdTicket = request.IdTicket,
                IdArticulo = request.IdArticulo,
                IdUsuario = request.IdUsuario,
                FechaVinculacion = DateTime.UtcNow
            };
            entity.IdUsuario = actor.Id;
            await _unitOfWork.Repository<global::HelpDesk.Domain.Models.TicketsArticulos>().AddAsync(entity);
            await _unitOfWork.Complete();
            return new ResponseDto<TicketsArticulosResponseDto>(201, new TicketsArticulosResponseDto
            {
                IdTicket = entity.IdTicket,
                IdArticulo = entity.IdArticulo,
                IdUsuario = entity.IdUsuario,
                FechaVinculacion = entity.FechaVinculacion
            }, "Registro creado.");
        }
        catch (BusinessException ex)
        {
            return new ResponseDto<TicketsArticulosResponseDto>(ex.Status, default, ex.Message);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new ResponseDto<TicketsArticulosResponseDto>(500, default, "No se pudo completar la operacion.");
        }
    }

}
