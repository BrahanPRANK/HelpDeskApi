using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.PoliticasAtencion.Dtos;
public class PoliticasAtencionInputDto
{
    [Required]
    [StringLength(10000)]
    public string Prioridad { get; set; } = "media";
    public int MinutosPrimeraRespuesta { get; set; }
    public int MinutosResolucion { get; set; }
    public bool Activo { get; set; } = true;
}
public class PoliticasAtencionResponseDto
{
    public Guid IdPolitica { get; set; }
    public string Prioridad { get; set; } = string.Empty;
    public int MinutosPrimeraRespuesta { get; set; }
    public int MinutosResolucion { get; set; }
    public bool Activo { get; set; }
}
