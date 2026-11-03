namespace HelpDesk.Domain.Dtos.TicketsArticulos;

public class TicketsArticulosInputDto
{
    public Guid IdTicket
    { get; set; }
    public Guid IdArticulo
    { get; set; }
    public Guid IdUsuario
    { get; set; }
}
