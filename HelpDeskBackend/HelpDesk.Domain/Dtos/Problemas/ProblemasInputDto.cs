using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Problemas;

public class ProblemasInputDto
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
    public string Estado
    { get; set; } = "abierto";
    public string? CausaRaiz
    { get; set; }
    public string? SolucionTemporal
    { get; set; }
    public string? SolucionDefinitiva
    { get; set; }
}
