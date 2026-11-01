using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.TicketsArticulos.Dtos;
public class TicketsArticulosInputDto
{
    public Guid IdTicket { get; set; }
    public Guid IdArticulo { get; set; }
    public Guid IdUsuario { get; set; }
}
public class TicketsArticulosResponseDto
{
    public Guid IdTicket { get; set; }
    public Guid IdArticulo { get; set; }
    public Guid IdUsuario { get; set; }
    public DateTime FechaVinculacion { get; set; }
}
