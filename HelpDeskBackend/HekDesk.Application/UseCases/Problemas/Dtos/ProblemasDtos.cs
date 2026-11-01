using System.ComponentModel.DataAnnotations;
namespace HelpDesk.Application.UseCases.Problemas.Dtos;
public class ProblemasInputDto
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
    public string Estado { get; set; } = "abierto";
    public string? CausaRaiz { get; set; }
    public string? SolucionTemporal { get; set; }
    public string? SolucionDefinitiva { get; set; }
}
public class ProblemasResponseDto
{
    public Guid IdProblema { get; set; }
    public Guid IdResponsable { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? CausaRaiz { get; set; }
    public string? SolucionTemporal { get; set; }
    public string? SolucionDefinitiva { get; set; }
    public DateTime FechaCreacion { get; set; }
}
