using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.PoliticasAtencion;

public class PoliticasAtencionInputDto
{
    [Required]
    [StringLength(10000)]
    public string Prioridad
    { get; set; } = "media";
    public int MinutosPrimeraRespuesta
    { get; set; }
    public int MinutosResolucion
    { get; set; }
    public bool Activo
    { get; set; } = true;
}
