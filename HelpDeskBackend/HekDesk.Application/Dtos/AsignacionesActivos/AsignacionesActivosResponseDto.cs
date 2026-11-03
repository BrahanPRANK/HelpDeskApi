namespace HelpDesk.Application.Dtos.AsignacionesActivos;

public class AsignacionesActivosResponseDto
{
    public Guid IdAsignacion
    { get; set; }
    public Guid IdActivo
    { get; set; }
    public Guid IdUsuarioResponsable
    { get; set; }
    public Guid IdUsuarioAsignador
    { get; set; }
    public DateTime FechaAsignacion
    { get; set; }
    public DateTime? FechaDevolucion
    { get; set; }
    public string? Observaciones
    { get; set; }
}
