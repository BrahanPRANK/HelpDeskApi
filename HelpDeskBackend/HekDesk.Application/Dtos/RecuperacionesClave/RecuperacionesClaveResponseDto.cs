namespace HelpDesk.Application.Dtos.RecuperacionesClave;

public class RecuperacionesClaveResponseDto
{
    public Guid IdRecuperacion
    { get; set; }
    public Guid IdUsuario
    { get; set; }
    public DateTime FechaSolicitud
    { get; set; }
    public DateTime FechaVencimiento
    { get; set; }
    public DateTime? FechaUso
    { get; set; }
}
