namespace HelpDesk.Domain.Dtos.HistorialTickets;

public class HistorialTicketsResponseDto
{
    public Guid IdHistorial
    { get; set; }
    public Guid IdTicket
    { get; set; }
    public Guid? IdUsuario
    { get; set; }
    public string Accion
    { get; set; } = string.Empty;
    public string? CampoModificado
    { get; set; }
    public string? ValorAnterior
    { get; set; }
    public string? ValorNuevo
    { get; set; }
    public string? Motivo
    { get; set; }
    public DateTime Fecha
    { get; set; }
}
