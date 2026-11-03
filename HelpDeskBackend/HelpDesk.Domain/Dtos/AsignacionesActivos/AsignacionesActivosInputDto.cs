namespace HelpDesk.Domain.Dtos.AsignacionesActivos;

public class AsignacionesActivosInputDto
{
    public Guid IdActivo
    { get; set; }
    public Guid IdUsuarioResponsable
    { get; set; }
    public string? Observaciones
    { get; set; }
}
