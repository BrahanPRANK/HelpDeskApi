using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Domain.Dtos.CategoriasConocimiento;

public class CategoriasConocimientoInputDto
{
    [Required]
    [StringLength(10000)]
    public string Nombre
    { get; set; } = "";
    public string? Descripcion
    { get; set; }
    public bool Activo
    { get; set; } = true;
}
