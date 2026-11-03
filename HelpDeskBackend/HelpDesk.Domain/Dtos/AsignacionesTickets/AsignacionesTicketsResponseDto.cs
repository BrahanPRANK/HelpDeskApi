namespace HelpDesk.Domain.Dtos.AsignacionesTickets;

public class AsignacionesTicketsResponseDto
{
    public Guid IdAsignacionTicket
    { get; set; }
    public Guid IdTicket
    { get; set; }
    public Guid IdEquipo
    { get; set; }
    public Guid? IdTecnico
    { get; set; }
    public Guid IdAsignador
    { get; set; }
    public DateTime FechaAsignacion
    { get; set; }
    public DateTime? FechaFin
    { get; set; }
    public string Motivo
    { get; set; } = string.Empty;
}
