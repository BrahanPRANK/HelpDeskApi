using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Cambios.Dtos;
public class CambiosInputDto
{
    public Guid IdResponsable { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Riesgo { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string PlanEjecucion { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string PlanReversion { get; set; } = "";
    public bool RequiereAprobacion { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public string? Resultado { get; set; }
}
public class CambiosResponseDto
{
    public Guid IdCambio { get; set; }
    public Guid IdSolicitante { get; set; }
    public Guid IdResponsable { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Riesgo { get; set; } = string.Empty;
    public string PlanEjecucion { get; set; } = string.Empty;
    public string PlanReversion { get; set; } = string.Empty;
    public bool RequiereAprobacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaFinalizacion { get; set; }
    public string? Resultado { get; set; }
    public DateTime FechaCreacion { get; set; }
}
