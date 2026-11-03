using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Cambios;

public class CambiosInputDto
{
    public Guid IdResponsable
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Titulo
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Descripcion
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string Riesgo
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string PlanEjecucion
    { get; set; } = "";
    [Required]
    [StringLength(10000)]
    public string PlanReversion
    { get; set; } = "";
    public bool RequiereAprobacion
    { get; set; }
    public DateTime? FechaProgramada
    { get; set; }
    public string? Resultado
    { get; set; }
}
