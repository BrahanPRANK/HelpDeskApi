namespace HelpDesk.Domain.Dtos.Notificaciones;

public class NotificacionesResponseDto
{
    public Guid IdNotificacion
    { get; set; }
    public Guid IdUsuario
    { get; set; }
    public Guid? IdTicket
    { get; set; }
    public string Titulo
    { get; set; } = string.Empty;
    public string Mensaje
    { get; set; } = string.Empty;
    public DateTime FechaCreacion
    { get; set; }
    public DateTime? FechaLectura
    { get; set; }
    public string ClaveEvento
    { get; set; } = string.Empty;
    public string TipoEvento
    { get; set; } = string.Empty;
}
