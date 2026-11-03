namespace HelpDesk.Domain.Dtos.RecuperacionesClave;

public class RecuperacionesClaveInputDto
{
    public Guid IdUsuario
    { get; set; }
    public DateTime FechaVencimiento
    { get; set; }
    public DateTime? FechaUso
    { get; set; }
}
