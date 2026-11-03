using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.Sedes;

public class SedesInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre
    { get; set; } = "";
    public string? Direccion
    { get; set; }
    public bool Activo
    { get; set; } = true;
}
