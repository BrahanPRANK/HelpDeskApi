namespace HelpDesk.Application.Dtos.TicketsArticulos;

public class TicketsArticulosResponseDto
{
    public Guid IdTicket
    { get; set; }
    public Guid IdArticulo
    { get; set; }
    public Guid IdUsuario
    { get; set; }
    public DateTime FechaVinculacion
    { get; set; }
}
