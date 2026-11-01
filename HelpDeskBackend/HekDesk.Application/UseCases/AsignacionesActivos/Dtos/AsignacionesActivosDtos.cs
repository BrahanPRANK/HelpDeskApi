using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.AsignacionesActivos.Dtos;
public class AsignacionesActivosInputDto
{
    public Guid IdActivo { get; set; }
    public Guid IdUsuarioResponsable { get; set; }
    public string? Observaciones { get; set; }
}
public class AsignacionesActivosResponseDto
{
    public Guid IdAsignacion { get; set; }
    public Guid IdActivo { get; set; }
    public Guid IdUsuarioResponsable { get; set; }
    public Guid IdUsuarioAsignador { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string? Observaciones { get; set; }
}
