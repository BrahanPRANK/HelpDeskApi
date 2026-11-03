using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Application.Dtos.Areas;

public class AreasInputDto
{
    public Guid IdSede
    { get; set; }
    [Required]
    [StringLength(10000)]
    public string Nombre
    { get; set; } = "";
    public bool Activo
    { get; set; } = true;
}
